import { Done } from '@mui/icons-material'
import { Grid, IconButton, Tooltip, Typography } from '@mui/material'
import { Box } from '@mui/system'
import React, { useEffect, useState } from 'react'
import { connect } from 'react-redux'
import { useNavigate } from 'react-router-dom'
import { postConfiguration } from '../../../api/configurationAPI'
import { translate } from '../../../lang'
import { alertTypes, openAlert } from '../../../state/alert/alertSlice'
import { inputDialogOpen } from '../../../state/inputDialog/inputDialogSlice'
import { selectLanguage } from '../../../state/language/languageSelectors'
import { selectIsAdmin } from '../../../state/user/userSelector'
import SectionTabs from './SectionTabs'

function CreateConfigurationView({ isAdmin, openAlert, openInputDialog, language }) {
    
    const navigate = useNavigate()

    const [configuration, setConfiguration] = useState({
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
    })
    
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

            postConfiguration(configurationName)
            .then(res => {
                openAlert(`${res}`, alertTypes.SUCCESS)
                navigate('/')
            })
            .catch(err => {
                openAlert(`Error: ${err}`, alertTypes.ERROR)
            })
        })
    }

    const renderCreatorBody = () => {
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
                    <Tooltip title={translate('finishCreation', language)}>
                        <IconButton 
                            variant="contained" 
                            onClick={handleFinishClicked}
                            >
                            <Done />
                        </IconButton>
                    </Tooltip>
                </Grid>
            </Grid>

            {renderCreatorBody()}
        </div>
    )
}
const mapStateToProps = (state) => ({
    isAdmin: selectIsAdmin(state),
    language: selectLanguage(state)
})
const mapDispatchToProps = {
    openAlert,
    openInputDialog: inputDialogOpen
}
export default connect(
    mapStateToProps, mapDispatchToProps
)(CreateConfigurationView)