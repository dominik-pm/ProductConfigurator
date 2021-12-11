import React from 'react'
import { Typography, Box, Button } from '@mui/material'
import { connect } from 'react-redux'
import { getIsOptionSelected, getOption, getOptionPrice } from '../../../../state/configuration/configurationSelectors'
import { selectOption } from '../../../../state/configuration/configurationSlice'
import './Option.css'

function Option({optionId, option, selected, price, selectOption}) {
    
    const disabled = false

    function handleClick() {
        selectOption(option.id)
    }
    
    return (
        <Button variant={selected ? "contained" : "outlined"} disabled={disabled} onClick={handleClick}>
            <Box className="Option">
                <Typography variant="h4">{option.name}</Typography>
                <Typography variant="body1">{option.description}</Typography>
                {price ? 
                    <Typography variant="body2">Price: {price}€</Typography>
                    :
                    <></>
                }
            </Box>
        </Button>
    )
}

const mapStateToProps = (state, ownProps) => {
    return {
        option: getOption(state, ownProps.optionId),
        selected: getIsOptionSelected(state, ownProps.optionId),
        price: getOptionPrice(state, ownProps.optionId)
    }
}
const mapDispatchToProps = {
    selectOption
}
export default connect(
    mapStateToProps,
    mapDispatchToProps
)(Option)