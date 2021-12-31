import React, { useEffect } from 'react'
import { connect } from 'react-redux'
import { Box } from '@mui/system'
import { Stack, Typography } from '@mui/material'
import PriceListItem from './PriceListItem'
import { getCurrentPrice, selectBasePrice, selectConfigurationId, selectConfigurationStatus, selectOptions } from '../../../../state/configuration/configurationSelectors'
import { translate } from '../../../../lang'
import { selectLanguage } from '../../../../state/language/languageSelectors'
import { fetchConfiguration, setSelectedOptions } from '../../../../state/configuration/configurationSlice'

function Summary({ configurationId, selectedOptions, status, loadedConfigurationId, basePrice, currentPrice, fetchConfiguration, setSelectedOptions, language }) {

    useEffect(() => {
        // if the selectedoptions or the loaded configuration changes, set the selected options again
        setSelectedOptions(selectedOptions)
    }, [loadedConfigurationId, selectedOptions, setSelectedOptions])

    useEffect(() => {
        if (configurationId !== loadedConfigurationId) {
            // no configuration or a different configuration is loaded -> load correct configuration
            fetchConfiguration(configurationId)
        }
    }, [configurationId, loadedConfigurationId, fetchConfiguration])

    function renderSummary() {
        if (status !== 'succeeded') {
            return (<></>)
        }

        return (
            <Box>
                <Typography variant="h3">
                    {translate('price', language)}: {currentPrice}€
                </Typography>

                <Stack spacing={1}>
                    <Typography>{translate('priceList', language)}</Typography>
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
                </Stack>
            </Box>
        )
    }

    return (
        renderSummary()
    )
}

const mapStateToProps = (state) => {
    return {
        status: selectConfigurationStatus(state),
        loadedConfigurationId: selectConfigurationId(state),
        options: selectOptions(state),
        basePrice: selectBasePrice(state),
        currentPrice: getCurrentPrice(state),
        language: selectLanguage(state)
    }
}
const mapDispatchToProps = {
    fetchConfiguration: fetchConfiguration,
    setSelectedOptions: setSelectedOptions
}
export default connect(
    mapStateToProps,
    mapDispatchToProps
)(Summary)
