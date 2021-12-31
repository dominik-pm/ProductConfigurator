import React from 'react'
import { connect } from 'react-redux'
import { Box, Grid, IconButton, Typography } from '@mui/material'
import { Edit, Preview } from '@mui/icons-material'
import { useNavigate } from 'react-router-dom'
import { saveConfigurationToStorage } from '../../state/configuration/configurationSlice'

function ConfigurationList({ configurations, isOrdered = false, isAdminView = false }) {

    const navigate = useNavigate()

    function handleEditClick(id, options) {
        navigate(`/configuration/${id}`)
        saveConfigurationToStorage(id, options)
    }

    function handleShowSummaryClicked() {
        
    }

    return (
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
                            <IconButton onClick={handleShowSummaryClicked}>
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