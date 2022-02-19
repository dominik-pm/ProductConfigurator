import { Delete, Done } from '@mui/icons-material'
import { FormControl, Grid, IconButton, InputLabel, ListItemText, MenuItem, OutlinedInput, Select, Tooltip, Typography } from '@mui/material'
import React, { useEffect } from 'react'
import { connect } from 'react-redux'
import { useNavigate } from 'react-router-dom'
import { languageNames, translate } from '../../../lang'
import { alertTypes, openAlert } from '../../../state/alert/alertSlice'
import { selectBuilderError, selectBuilderInputLanguage, selectBuilderStatus } from '../../../state/configurationBuilder/builderSelectors'
import { changeInputLanguage, finishConfigurationBuild, loadingHandled, resetBuild, saveBuilderToStorage } from '../../../state/configurationBuilder/builderSlice'
import { confirmDialogOpen } from '../../../state/confirmationDialog/confirmationSlice'
import { selectLanguage } from '../../../state/language/languageSelectors'
import { selectIsAdmin } from '../../../state/user/userSelector'
import ConfigurationProperties from './ConfigurationProperties'
import ImageSelect from './ImageSelect'
import CreateModel from './Model/CreateModel'
import SectionTabs from './SectionTabs'

function ConfigurationBuilderView({ isAdmin, status, error, inputLanguage, openAlert, openConfirmDialog, changeInputLanguage, saveBuilderToStorage, finish, reset, loadingHandled, language }) {
    
    const autoSave = true
    const autoSaveInterval = 10000

    const navigate = useNavigate()

    // open alert and navigate to the home page if the configuration is created
    useEffect(() => {
        if (status === 'succeeded') {
            loadingHandled()
            openAlert(translate('successConfigurationCreated', language), alertTypes.SUCCESS)
            navigate('/')
        }
    }, [status, navigate, loadingHandled, openAlert])

    // open alert if there is an error
    useEffect(() => {
        if (error) {
            loadingHandled()
            console.log('error:', error)
            openAlert(`Error: ${error}`, alertTypes.ERROR)
        }
    }, [error, loadingHandled, openAlert])

    // if the user is not an admin, navigate to the home page
    useEffect(() => {
        if (!isAdmin) {
            navigate('/')
        }
    }, [isAdmin, navigate])


    // start the auto save when the component is initialized
    useEffect(() => {
        if (!autoSave) return

        let saveBuilderInterval = setInterval(() => {
            console.log('Saving Builder...')
            saveBuilderToStorage()
        }, autoSaveInterval)

        // clear interval when the component is unmounted
        return () => {clearInterval(saveBuilderInterval)}
    }, [saveBuilderToStorage])

    const handleFinishClicked = () => {
        openConfirmDialog(translate('createConfiguration', language), {}, null, () => {
            finish()
        })
    }

    const handleResetClicked = () => {
        openConfirmDialog(translate('resetBuildConfirmation', language), {}, null, () => {
            reset()
        })
    }

    const handleChangeInputLanguage = (event) => {
        const newLang = event.target.value
        changeInputLanguage(newLang)
    }

    const renderBuilderBody = () => {
        return (
            <Grid container direction="column" gap={2}>
                <ConfigurationProperties></ConfigurationProperties>
                <ImageSelect></ImageSelect>
                <CreateModel></CreateModel>
                <SectionTabs></SectionTabs>
            </Grid>
        )
    }

    return (
        <div>
            <Grid container justifyContent="flex-end">
                {/* Header */}
                <Grid item sx={{flexGrow: 1}}>
                    <Typography variant="h2">{translate('createNewConfiguration', language)}</Typography>
                </Grid>

                {/* Actions */}
                <Grid item display="flex" alignItems="center" sx={{paddingTop: 2, justifySelf: 'flex-end'}}>
                    
                    <FormControl sx={{width: 150}} >
                        <InputLabel id="select-input-lang-label">Input Language</InputLabel>
                        <Select
                            labelId='select-input-lang-label'
                            value={inputLanguage}
                            autoWidth
                            onChange={handleChangeInputLanguage}
                            input={<OutlinedInput label='Input Language' />}
                        >
                            {Object.keys(languageNames).map((lang, index) => (
                                <MenuItem sx={{width: 150}} key={index} value={languageNames[lang]}>
                                    <ListItemText primary={lang}></ListItemText>
                                </MenuItem>
                            ))}
                        </Select>
                    </FormControl>

                    <Tooltip title={translate('finishBuild', language)}>
                        <IconButton 
                            variant="contained" 
                            onClick={handleFinishClicked}
                            >
                            <Done />
                        </IconButton>
                    </Tooltip>

                    <Tooltip title={translate('resetBuild', language)}>
                        <IconButton 
                            variant="contained" 
                            onClick={handleResetClicked}
                            >
                            <Delete />
                        </IconButton>
                    </Tooltip>
                </Grid>

                {/* Language Select */}
                <Grid item container>
                </Grid>

                {/* Builder */}
                {renderBuilderBody()}
            </Grid>
        </div>
    )
}
const mapStateToProps = (state) => ({
    isAdmin: selectIsAdmin(state),
    status: selectBuilderStatus(state),
    error: selectBuilderError(state),
    inputLanguage: selectBuilderInputLanguage(state),
    language: selectLanguage(state)
})
const mapDispatchToProps = {
    openAlert,
    openConfirmDialog: confirmDialogOpen,
    changeInputLanguage,
    saveBuilderToStorage,
    finish: finishConfigurationBuild,
    reset: resetBuild,
    loadingHandled: loadingHandled

}
export default connect(
    mapStateToProps, mapDispatchToProps
)(ConfigurationBuilderView)



// export class ConfigurationBuilderView extends Component {

//     constructor(props) {
//         super(props)
//     }

//     componentDidUpdate() {
//         this.checkState()
//     }

//     componentDidMount() {
//         this.checkState()
//         this.saveBuilderInterval = setInterval(this.saveBuilder, 10000)
//     }

//     componentWillUnmount() {
//         clearInterval(this.saveBuilderInterval)
//     }

//     checkState = () => {
//         console.log('CHECKING STATE')
//         const {navigate} = this.props

//         const { isAdmin, status, error, openAlert, loadingHandled } = this.props

//         if (error) {
//             loadingHandled()
//             console.log('error:', error)
//             openAlert(`Error: ${error}`, alertTypes.ERROR)
//         }

//         if (status === 'succeeded') {
//             loadingHandled()
//             openAlert(`Successfully created a new configuration!`, alertTypes.SUCCESS)
//             navigate('/')
//         }

//         console.log(isAdmin)
//         if (!isAdmin) {
//             console.log('not admin')
//             console.log(navigate)
//             navigate('/')
//         }
//     }

//     saveBuilder = () => {
//         const { saveBuilderToStorage } = this.props
//         console.log('Saving Builder...')
//         saveBuilderToStorage()
//     }

//     handleFinishClicked = () => {
//         const { openInputDialog, finish, language } = this.props

//         const data = {
//             configurationName: {name: translate('configurationName', language), value: '' }
//         }
//         const title = translate('finishConfiguration', language)

//         openInputDialog(title, data, (data) => {
//             const configurationName = data.configurationName.value

//             finish(configurationName)
//         })
//     }

//     handleResetClicked = () => {
//         const { reset, language } = this.props
        
//         this.openConfirmDialog(translate('resetBuildConfirmation', language), {}, null, () => {
//             reset()
//         })
//     }

//     renderBuilderBody = () => {
//         return (
//             <Grid container direction="column" gap={2}>
//                 <ConfigurationProperties></ConfigurationProperties>
//                 <CreateModel></CreateModel>
//                 <SectionTabs></SectionTabs>
//             </Grid>
//         )
//     }

//     render() {
//         const { language } = this.props
        
//         return (
//             <div>
//                 <Grid container justifyContent="flex-end">
//                     <Grid item sx={{flexGrow: 1}}>
//                         <Typography variant="h2">{translate('createNewConfiguration', language)}</Typography>
//                     </Grid>

//                     <Grid item sx={{paddingTop: 2, justifySelf: 'flex-end'}}>
//                         <Tooltip title={translate('finishBuild', language)}>
//                             <IconButton 
//                                 variant="contained" 
//                                 onClick={this.handleFinishClicked}
//                                 >
//                                 <Done />
//                             </IconButton>
//                         </Tooltip>
//                         <Tooltip title={translate('resetBuild', language)}>
//                             <IconButton 
//                                 variant="contained" 
//                                 onClick={this.handleResetClicked}
//                                 >
//                                 <Delete />
//                             </IconButton>
//                         </Tooltip>
//                     </Grid>
//                 </Grid>

//                 {this.renderBuilderBody()}
//             </div>
//         )
//     }
// }

// const mapStateToProps = (state) => ({
//     isAdmin: selectIsAdmin(state),
//     status: selectBuilderStatus(state),
//     error: selectBuilderError(state),
//     language: selectLanguage(state)
// })
// const mapDispatchToProps = {
//     openAlert,
//     openInputDialog: inputDialogOpen,
//     openConfirmDialog: confirmDialogOpen,
//     saveBuilderToStorage: saveBuilderToStorage,
//     finish: finishConfigurationBuild,
//     reset: resetBuild,
//     loadingHandled: loadingHandled
// }
// const ConfigurationBuilderViewConnected = connect(
//     mapStateToProps, 
//     mapDispatchToProps
// )(ConfigurationBuilderView)

// // export default ConfigurationBuilderViewConnected

// function WithNavigate(props) {
//     const navigate = useNavigate()
//     return <ConfigurationBuilderViewConnected {...props} navigate={navigate}></ConfigurationBuilderViewConnected>
// }
// export default WithNavigate