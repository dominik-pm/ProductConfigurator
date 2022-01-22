import { Delete, Done } from '@mui/icons-material'
import { Grid, IconButton, Tooltip, Typography } from '@mui/material'
import { Box } from '@mui/system'
import React, { useEffect } from 'react'
import { connect } from 'react-redux'
import { useNavigate } from 'react-router-dom'
import { translate } from '../../../lang'
import { alertTypes, openAlert } from '../../../state/alert/alertSlice'
import { selectBuilderError, selectBuilderStatus } from '../../../state/configurationBuilder/builderSelectors'
import { finishConfigurationBuild, loadingHandled, resetBuild } from '../../../state/configurationBuilder/builderSlice'
import { confirmDialogOpen } from '../../../state/confirmationDialog/confirmationSlice'
import { inputDialogOpen } from '../../../state/inputDialog/inputDialogSlice'
import { selectLanguage } from '../../../state/language/languageSelectors'
import { selectIsAdmin } from '../../../state/user/userSelector'
import SectionTabs from './SectionTabs'

function CreateConfigurationView({ isAdmin, status, error, openAlert, openInputDialog, openConfirmDialog, finish, reset, loadingHandled, language }) {
    
    const navigate = useNavigate()

    // const configuration = {
        // name: 'Car',
        // description: 'automobile',
        // image: '1.jpg',
        // options: [
        //     {
        //         id: 'BLUE',
        //         name: 'Blue',
        //         description: 'A blue color',
        //         image: ''
        //     }
        // ],
        // optionSections: [
        //     {
        //         id: 'EXTERIOR',
        //         name: 'Exterior',
        //         optionGroupIds: [
        //             'COLOR_GROUP'
        //         ]
        //     }
        // ],
        // optionGroups: [
        //     {
        //         id: 'COLOR_GROUP',
        //         name: 'Color',
        //         description: 'the exterior color of the car',
        //         optionIds: [
        //             'BLUE', 'YELLOW', 'GREEN'
        //         ],
        //         required: true
        //     }
        // ],
        // rules: {
        //     basePrice: 10000,
        //     defaultOptions: ['BLUE'],
        //     replacementGroups: {
        //         COLOR_GROUP: [
        //             'BLUE'
        //         ]
        //     },
        //     groupRequirements: {
        //         PANORAMATYPE_GROUP: ['PANORAMA_GROUP']
        //     },
        //     requirements: {
        //         D150: ['DIESEL']
        //     },
        //     incompatibilites: {
        //         PANORAMAROOF: ['PETROL']
        //     },
        //     priceList: {
        //         D150: 8000
        //     }
        // }
    // }
    

    if (error) {
        loadingHandled()
        console.log('error:', error)
        // TODO: alert is opened twice (but only called once)
        openAlert(`Error: ${error}`, alertTypes.ERROR)
    }

    if (status === 'succeeded') {
        loadingHandled()
        openAlert(`Successfully created a new configuration!`, alertTypes.SUCCESS)
        navigate('/')
    }

    useEffect(() => {
        if (!isAdmin) {
            navigate('/')
        }
    }, [isAdmin, navigate])


    const handleFinishClicked = () => {
        const data = {
            configurationName: {name: translate('configurationName', language), value: '' }
        }
        const title = translate('finishConfiguration', language)

        openInputDialog(title, data, (data) => {
            const configurationName = data.configurationName.value

            finish(configurationName)
        })
    }

    const handleResetClicked = () => {
        openConfirmDialog(translate('resetBuildConfirmation', language), {}, null, () => {
            reset()
        })
    }

    const renderBuilderBody = () => {
        return (
            <Grid container>
                <SectionTabs></SectionTabs>
            </Grid>
        )
    }

    return (
        <div>
            <Grid container justifyContent="flex-end">
                <Box sx={{flexGrow: 1}}>
                    <Typography variant="h2">Create a new Configuration</Typography>
                </Box>

                <Grid item sx={{paddingTop: 2, justifySelf: 'flex-end'}}>
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
            </Grid>

            {renderBuilderBody()}
        </div>
    )
}
const mapStateToProps = (state) => ({
    isAdmin: selectIsAdmin(state),
    status: selectBuilderStatus(state),
    error: selectBuilderError(state),
    language: selectLanguage(state)
})
const mapDispatchToProps = {
    openAlert,
    openInputDialog: inputDialogOpen,
    openConfirmDialog: confirmDialogOpen,
    finish: finishConfigurationBuild,
    reset: resetBuild,
    loadingHandled: loadingHandled

}
export default connect(
    mapStateToProps, mapDispatchToProps
)(CreateConfigurationView)