import React, { useState } from 'react'
import { connect } from 'react-redux'
import { Box, Grid, IconButton, Typography } from '@mui/material'
import { Edit, Preview } from '@mui/icons-material'
import { useNavigate } from 'react-router-dom'
import { saveConfigurationToStorage } from '../../state/configuration/configurationSlice'
import Summary from '../configuration/Configurator/SidePanel/Summary'

function ConfigurationList({ configurations, isOrdered = false, isAdminView = false }) {

    const [summaryConfigId, setSummaryConfigId] = useState(null)
    const [summarySelectedOptions, setSummarySelectedOptions] = useState(null)

    const navigate = useNavigate()

    function handleEditClick(id, options) {
        navigate(`/configuration/${id}`)
        saveConfigurationToStorage(id, options)
    }

    function handleShowSummaryClicked(id, options) {
        setSummarySelectedOptions(options)
        setSummaryConfigId(id)
    }

    return (
        <Box>
            <Grid container>
                {configurations.map((config, index) => {
                    return (
                        <Box key={index} margin={2}>
                            <Box display="flex" alignItems="center">
                                <Typography variant="body1">{config.savedName}</Typography>
                                <IconButton onClick={() => handleEditClick(config.id, config.options)}>
                                    <Edit></Edit>
                                </IconButton>
                                {isOrdered ?
                                <IconButton onClick={() => handleShowSummaryClicked(config.id, config.options)}>
                                    <Preview></Preview>
                                </IconButton>
                                : ''}
                            </Box>

                            <Typography variant="body2">{config.name}</Typography>

                            <Typography variant="body2">{new Date(config.date).toLocaleTimeString()}</Typography>

                            {isAdminView ? 
                            <Typography variant="body2">From: {config.userName}</Typography>
                            : ''}
                        </Box>
                    )
                })}
            </Grid>

            {summaryConfigId !== null ? 
            <Summary configurationId={summaryConfigId} selectedOptions={summarySelectedOptions}></Summary>
            : ''}
        </Box>
    )
}

const mapStateToProps = (state) => ({
    
})
const mapDispatchToProps = {
    
}
export default connect(
    mapStateToProps, 
    mapDispatchToProps
)(ConfigurationList)