import { Done, RestartAlt, SaveAs } from '@mui/icons-material'
import { Box, Grid, IconButton, Tooltip, Typography } from '@mui/material'
import React from 'react'
import { connect } from 'react-redux'
import { translate } from '../../../lang'
import { selectConfigurationDescription, selectConfigurationName } from '../../../state/configuration/configurationSelectors'
import { resetActiveConfiguration } from '../../../state/configuration/configurationSlice'
import { confirmDialogOpen } from '../../../state/confirmationDialog/confirmationSlice'
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

function Configurator({ configurationName, configurationDescription, isLoading, resetConfig, openConfirm, language }) {

    function handleSaveClicked() {
        console.log('save configuration pressed')
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
    language: selectLanguage(state)
})
const mapDispatchToProps = {
    resetConfig: resetActiveConfiguration,
    openConfirm: confirmDialogOpen
}
export default connect(
    mapStateToProps,
    mapDispatchToProps
)(Configurator)
