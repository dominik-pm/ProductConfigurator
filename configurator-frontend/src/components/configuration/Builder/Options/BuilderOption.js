import { Box, Typography } from '@mui/material'
import React from 'react'
import { connect } from 'react-redux'
import { getBuilderOptionById } from '../../../../state/configurationBuilder/builderSelectors'

function BuilderOption({ optionId, option }) {
    return (
        <Box>
            <Typography variant="body1">{option.name}</Typography>
            <Typography variant="body2">{option.description}</Typography>
        </Box>
    )
}

const mapStateToProps = (state, ownProps) => ({
    option: getBuilderOptionById(state, ownProps.optionId)
})
const mapDispatchToProps = {
    
}
export default connect(
    mapStateToProps, 
    mapDispatchToProps
)(BuilderOption)