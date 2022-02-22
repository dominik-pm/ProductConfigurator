import { Typography } from '@mui/material'
import { green, red } from '@mui/material/colors'
import { Box } from '@mui/system'
import React from 'react'
import { connect } from 'react-redux'
import { getOption } from '../../../../state/configuration/configurationSelectors'

function OptionListItem({ optionId, highlight, name, description, image }) {
    let bgCol = 'none'
    if (highlight === 'add')    bgCol = green[500]
    if (highlight === 'remove') bgCol = red[500]
    
    return (
        <Box padding={1} margin={1} sx={{backgroundColor: bgCol}}>
            <Typography variant="body1">{name}</Typography>
        </Box>
    )
}

const mapStateToProps = (state, ownProps) => {
    const option = getOption(state, ownProps.optionId)
    return {
        name: option.name,
        description: option.description,
        image: option.image
    }
}
const mapDispatchToProps = {}
export default connect(
    mapStateToProps, 
    mapDispatchToProps
)(OptionListItem)
