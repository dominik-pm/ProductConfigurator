import React from 'react'
import { connect } from 'react-redux'
import { Box } from '@mui/system'
import { Accordion, AccordionSummary, AccordionDetails, Stack, Typography } from '@mui/material'
import PriceListItem from './PriceListItem'
import { ExpandMoreOutlined } from '@mui/icons-material'
import { getBasePrice, getCurrentPrice, getOption, getOptionPrice, getOptions, getSelectedOptions } from '../../../../state/configuration/configurationSelectors'

function Summary({state, basePrice, selectedOptions, currentPrice }) {

    // const { options, selectedOptions, rules } = useSelector(state => state.configuration)

    return (
        <Box>
            <Typography variant="h3">
                Price: {currentPrice}€
            </Typography>

            <Stack spacing={1}>
                
                <Accordion>
                    <AccordionSummary
                        expandIcon={<ExpandMoreOutlined />}
                        aria-controls="panel1a-content"
                        id="panel1a-header"
                    >
                        <Typography>Price List</Typography>
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
                                name={getOption(state, optionId).name /*options.find(o => o.id === optionId).name*/}
                                price={getOptionPrice(state, optionId)}
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
        options: getOptions(state),
        basePrice: getBasePrice(state),
        selectedOptions: getSelectedOptions(state),
        currentPrice: getCurrentPrice(state)
    }
}
const mapDispatchToProps = {
    
}
export default connect(
    mapStateToProps,
    mapDispatchToProps
)(Summary)
