import { Grid, Typography } from '@mui/material'
import React from 'react'
import { connect } from 'react-redux'
import { selectConfigurationDescription, selectConfigurationName } from '../../../state/configuration/configurationSelectors'
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

function Configurator({ configurationName, configurationDescription, isLoading }) {

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
                <Typography variant="h1">Configure your {configurationName}</Typography>
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
    }
}
const mapDispatchToProps = {
    
}
export default connect(
    mapStateToProps,
    mapDispatchToProps
)(Configurator)
