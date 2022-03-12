import { ListItemText } from '@mui/material'
import React from 'react'
import { connect } from 'react-redux'
import { getOption } from '../../../../state/configuration/configurationSelectors'

function ModelOptionText({ name, description }) {

    return (
        <ListItemText
            primary={name}
            secondary={description}
        ></ListItemText>
    )
}

const mapStateToProps = (state, ownProps) => {
    const option = getOption(state, ownProps.optionId)

    return {
        name: option.name,
        description: option.description
    }
}
const mapDispatchToProps = {

}
export default connect(
    mapStateToProps, 
    mapDispatchToProps
)(ModelOptionText)
