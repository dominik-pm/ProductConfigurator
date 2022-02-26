import { ListItemText, Typography } from '@mui/material'
import { Box } from '@mui/system'
import React from 'react'
// import { useSelector } from 'react-redux'
import { connect } from 'react-redux'
import { extractNameFromOption, extractProductNumberFromOption, getOption, getOptionPrice } from '../../../../state/configuration/configurationSelectors'

function PriceListItem({state, optionId, name, price}) {
    // console.log('---------')
    // console.log(name + ' rendered new')
    // console.log('---------')
    let productNumber = ''

    if (optionId) {
        const option = getOption(state, optionId)
        name = extractNameFromOption(option)
        productNumber = extractProductNumberFromOption(option)
        price = getOptionPrice(state, optionId)
    }
    
    return (
        <Box display="flex" justifyContent="space-between" sx={{width: '100%'}}>
            <Typography variant="body2">
                {name}{productNumber ? ` (${productNumber})` : ''}{/* {{price ? `: ${price}€` : ''}} */}
            </Typography>
            {price ? 
            <Typography variant="body2">
                {price}€
            </Typography>
            : ''}
        </Box>
    )
}


const mapStateToProps = (state, ownProps) => {
    return {
        ...ownProps,
        state
    }
    // if (!ownProps.optionId) {
    //     console.log('|||||||||||||||||||\nbaseprice rendering new\n|||||||||||||||||||')
    //     return {
    //         ...ownProps
    //     }
    // }

    // return {
    //     name: getOption(state, ownProps.optionId).name,
    //     price: getOptionPrice(state, ownProps.optionId)
    // }
}
const mapDispatchToProps = {
    
}
export default connect(
    mapStateToProps,
    mapDispatchToProps
)(PriceListItem)
