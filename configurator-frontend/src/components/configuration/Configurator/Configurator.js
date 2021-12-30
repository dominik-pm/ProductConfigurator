import { Done, RestartAlt, SaveAs } from '@mui/icons-material'
import { Box, Grid, IconButton, Tooltip, Typography } from '@mui/material'
import React from 'react'
import { connect } from 'react-redux'
import { requestSaveConfiguration } from '../../../api/userAPI'
import { translate } from '../../../lang'
import { selectConfigurationDescription, selectConfigurationId, selectConfigurationName, selectSelectedOptions } from '../../../state/configuration/configurationSelectors'
import { resetActiveConfiguration } from '../../../state/configuration/configurationSlice'
import { confirmDialogOpen } from '../../../state/confirmationDialog/confirmationSlice'
import { inputDialogOpen } from '../../../state/inputDialog/inputDialogSlice'
import { selectLanguage } from '../../../state/language/languageSelectors'
import Loader from '../../Loader'

import OptionTabs from './OptionTabs'
// import Summary from './SidePanel/Summary'


/*

optionGroups: [
    {
        id: 'COLOR_GROUP',
        name: 'Color',
        description: 'the exterior color of the car',
        optionIds: [
            'BLUE', 'YELLOW', 'GREEN'
        ]
    }
]

*/

function Configurator({ configurationName, configurationDescription, configurationId, selectedOptions, isLoading, resetConfig, openConfirm, openInputDialog, language }) {

    function handleSaveClicked() {
        const data = {
            configurationName: {name: translate('configurationName', language), value: '' }
        }
        const title = translate('saveConfiguration', language)

        openInputDialog(title, data, () => {
            const configuration = {
                configurationId,
                selectedOptions
            }
            requestSaveConfiguration(title, configuration)
            .then(res => {
                // TODO: display notification
                console.log(res)
            })
            .catch(err => {
                // TODO: display notification
                console.log(err)
            })
        })
    }

    function handleResetClicked() {
        openConfirm(translate('resetConfigurationPrompt', language), {}, () => {
            resetConfig()
        })
    }

    function handleFinishClicked() {
        console.log('finish configuration pressed')
    }

    function renderConfiguratorBody() {

        return (
            // <Grid container spacing={2}>
            //     <Grid item xs={12} md={3}>
            //         <Summary></Summary>
            //     </Grid>

            //     <Grid item xs={12} md={9}>
            //         <OptionTabs></OptionTabs>
            //     </Grid>
            // </Grid>
            <Grid container>
                <OptionTabs></OptionTabs>
            </Grid>

        )
    }

    return (
        <div>
            <Grid container>
                <Box sx={{flexGrow: 1}}>
                    <Typography variant="h2">{translate('configureYour', language)} {configurationName}</Typography>
                    <Typography variant="subtitle1">{configurationDescription}</Typography>
                </Box>
                <Grid sx={{paddingTop: 2}}>

                    <Tooltip title={translate('saveConfiguration', language)}>
                        <IconButton 
                            variant="contained" 
                            onClick={handleSaveClicked}
                            >
                            <SaveAs />
                        </IconButton>
                    </Tooltip>

                    <Tooltip title={translate('resetConfiguration', language)}>
                        <IconButton
                            variant="contained" 
                            onClick={handleResetClicked}
                        >
                            <RestartAlt />
                        </IconButton>
                    </Tooltip>

                    <Tooltip title={translate('finishConfiguration', language)}>
                        <IconButton 
                            variant="contained" 
                            onClick={handleFinishClicked}
                            >
                            <Done />
                        </IconButton>
                    </Tooltip>
                </Grid>
            </Grid>
            {
                isLoading ? 
                <Loader></Loader>
                 :
                renderConfiguratorBody()
            }
        </div>
    )
}

const mapStateToProps = (state) => ({
    configurationName: selectConfigurationName(state),
    configurationDescription: selectConfigurationDescription(state),
    configurationId: selectConfigurationId(state),
    selectedOptions: selectSelectedOptions(state),
    language: selectLanguage(state)
})
const mapDispatchToProps = {
    resetConfig: resetActiveConfiguration,
    openConfirm: confirmDialogOpen,
    openInputDialog: inputDialogOpen
}
export default connect(
    mapStateToProps,
    mapDispatchToProps
)(Configurator)
