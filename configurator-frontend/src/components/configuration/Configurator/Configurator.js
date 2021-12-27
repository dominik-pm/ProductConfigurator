import { Grid, Typography } from '@mui/material'
import React from 'react'
import { connect } from 'react-redux'
import { translate } from '../../../lang'
import { selectConfigurationDescription, selectConfigurationName } from '../../../state/configuration/configurationSelectors'
import { selectLanguage } from '../../../state/language/languageSelectors'
import Loader from '../../Loader'

import OptionTabs from './OptionTabs'
import Summary from './SidePanel/Summary'


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

function Configurator({ configurationName, configurationDescription, isLoading, language }) {

    function render() {

        return (
            <Grid container spacing={2}>
                <Grid item xs={12} md={3}>
                    <Summary></Summary>
                </Grid>

                <Grid item xs={12} md={9}>
                    <OptionTabs></OptionTabs>
                </Grid>
            </Grid>
        )
    }

    return (
        <div>
            <div>
                <Typography variant="h1">{translate('configureYour', language)} {configurationName}</Typography>
                <Typography variant="subtitle1">{configurationDescription}</Typography>
            </div>
            {
                isLoading ? 
                <Loader></Loader>
                 :
                render()
            }
        </div>
    )
}

const mapStateToProps = (state) => {
    return {
        configurationName: selectConfigurationName(state),
        configurationDescription: selectConfigurationDescription(state),
        language: selectLanguage(state)
    }
}
const mapDispatchToProps = {
    
}
export default connect(
    mapStateToProps,
    mapDispatchToProps
)(Configurator)
