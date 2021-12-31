import React from 'react'
import { connect } from 'react-redux'
import { Box, Grid, Typography } from '@mui/material'

function ConfigurationList({ configurations, isOrdered = false, isAdminView = false }) {
    return (
        <Grid container>
            {configurations.map((config, index) => {
                return (
                    <Box key={index} margin={2}>
                        <Typography variant="body1">{config.savedName}</Typography>
                        <Typography variant="body2">{config.name}</Typography>
                        <Typography variant="body2">{new Date(config.date).toLocaleTimeString()}</Typography>
                        {isAdminView ? 
                        <Typography variant="body2">From: {config.userName}</Typography>
                        : ''
                    }
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