import React from 'react'
import { Typography, Box, Button, Tooltip, Zoom } from '@mui/material'
import { connect } from 'react-redux'
import { clickedOption } from '../../../../state/configuration/configurationSlice'
import './Option.css'
import { getIsOptionSelectable, getIsOptionSelected, getOption, getOptionName, getOptionPrice } from '../../../../state/configuration/configurationSelectors'
import { translate } from '../../../../lang'
import { selectLanguage } from '../../../../state/language/languageSelectors'

function Option({ optionId, clickedOption, option, selected, price, selectable, disabledReason, problematicOptions, language }) {

    const disabled = !selectable

    function handleClick() {
        clickedOption(optionId)
    }
    
    return (
        <Tooltip 
            title={disabled ? `${translate(disabledReason, language)}: ${problematicOptions.join(', ')}` : ''} 
            placement="top" 
            TransitionComponent={Zoom}
            arrow
            enterTouchDelay={200}
        >
            <Box >

            <Button style={{width: '100%', height: '100%'}} variant={selected ? "contained" : "outlined"} disabled={disabled} onClick={handleClick}>
                <Box className="Option">
                    <Typography variant="h4">{option.name}</Typography>
                    <Typography variant="body1">{option.description}</Typography>
                    {price ? 
                        <Typography variant="body2">{translate('price', language)}: {price}€</Typography>
                        :
                        <></>
                    }
                </Box>
            </Button>

            </Box>
        </Tooltip>
    )
}

const mapStateToProps = (state, ownProps) => {
    const selectableError = getIsOptionSelectable(state, ownProps.optionId)
    let isSelectable = true
    let disabledReason = ''
    let problematicOptions = []

    if (selectableError) {
        isSelectable = false
        disabledReason = selectableError[0]
        problematicOptions = selectableError[1].map(optionId => getOptionName(state, optionId))
    }

    return {
        option: getOption(state, ownProps.optionId),
        selected: getIsOptionSelected(state, ownProps.optionId),
        price: getOptionPrice(state, ownProps.optionId),
        selectable: isSelectable,
        disabledReason,
        problematicOptions,
        language: selectLanguage(state)
    }
}
const mapDispatchToProps = {
    clickedOption
}
export default connect(
    mapStateToProps,
    mapDispatchToProps
)(Option)