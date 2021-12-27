import React from 'react'
import { connect } from 'react-redux'
import { Box } from '@mui/system'
import { Accordion, AccordionSummary, AccordionDetails, Stack, Typography } from '@mui/material'
import PriceListItem from './PriceListItem'
import { ExpandMoreOutlined } from '@mui/icons-material'
import { getCurrentPrice, /*getOption, getOptionPrice, */selectBasePrice, selectOptions, selectSelectedOptions } from '../../../../state/configuration/configurationSelectors'
import { translate } from '../../../../lang'
import { selectLanguage } from '../../../../state/language/languageSelectors'

function Summary({state, basePrice, selectedOptions, currentPrice, language }) {

    // const { options, selectedOptions, rules } = useSelector(state => state.configuration)

    return (
        <Box>
            <Typography variant="h3">
                {translate('price', language)}: {currentPrice}€
            </Typography>

            <Stack spacing={1}>
                
                <Accordion>
                    <AccordionSummary
                        expandIcon={<ExpandMoreOutlined />}
                        aria-controls="panel1a-content"
                        id="panel1a-header"
                    >
                        <Typography>{translate('priceList', language)}</Typography>
                    </AccordionSummary>
                    <AccordionDetails>

                        <PriceListItem
                            name='Base Price'
                            price={basePrice}
                        >
                        </PriceListItem>

                        {selectedOptions.map((optionId, index) => (
                            <PriceListItem
                                key={index}
                                optionId={optionId}
                                name={optionId}
                                price={0}
                            >
                            </PriceListItem>
                        ))}

                    </AccordionDetails>
                </Accordion>

            </Stack>
        </Box>
    )
}

const mapStateToProps = (state) => {
    return {
        state,
        options: selectOptions(state),
        basePrice: selectBasePrice(state),
        selectedOptions: selectSelectedOptions(state),
        currentPrice: getCurrentPrice(state),
        language: selectLanguage(state)
    }
}
const mapDispatchToProps = {
    
}
export default connect(
    mapStateToProps,
    mapDispatchToProps
)(Summary)
