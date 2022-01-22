import { Box, Typography } from '@mui/material'
import React from 'react'
import { connect } from 'react-redux'

function BuilderOption({ optionId }) {
    return (
        <Box>
            <Typography variant="body1">{optionId}</Typography>
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
)(BuilderOption)