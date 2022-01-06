import React, { useEffect } from 'react'
import { connect } from 'react-redux'
import { Box } from '@mui/system'
import { Accordion, AccordionDetails, AccordionSummary, Divider, Stack, Typography } from '@mui/material'
import PriceListItem from './PriceListItem'
import { getCurrentPrice, selectBasePrice, selectConfigurationId, selectConfigurationStatus, selectOptionGroups, selectOptions, selectOptionSections } from '../../../../state/configuration/configurationSelectors'
import { translate } from '../../../../lang'
import { selectLanguage } from '../../../../state/language/languageSelectors'
import { fetchConfiguration, setSelectedOptions } from '../../../../state/configuration/configurationSlice'
import { ExpandMoreOutlined } from '@mui/icons-material'

function Summary({ configurationId, selectedOptions, status, loadedConfigurationId, sections, groups, basePrice, currentPrice, fetchConfiguration, setSelectedOptions, language }) {

    useEffect(() => {
        // if the selectedoptions or the loaded configuration changes, set the selected options again
        console.log('summary: setting selected options')
        setSelectedOptions(selectedOptions)
    }, [loadedConfigurationId, selectedOptions, setSelectedOptions])

    useEffect(() => {
        if (configurationId !== loadedConfigurationId) {
            // no configuration or a different configuration is loaded -> load correct configuration
            console.log('summary: fetching configuration')
            fetchConfiguration(configurationId)
        }
    }, [configurationId, loadedConfigurationId, fetchConfiguration])


    function renderSectionContent(section) {
        const sectionGroups = groups.filter(g => section.optionGroupIds.includes(g.id))
        const groupOptions = (group) => selectedOptions.filter(o => group.optionIds.includes(o))

        return (
            sectionGroups.map((group, index) => (
                <Box key={index}>
                    {groupOptions(group).length > 0 ?
                        <>
                            <Typography vairant="body1">{group.name}</Typography>
                            
                            {groupOptions(group).map((option, index) => (
                                <PriceListItem
                                key={index}
                                optionId={option}
                                >
                                </PriceListItem>
                            ))}
        
                            <Divider sx={{marginTop: 1, marginBottom: 1}} />

                        </> : ''
                    }
                </Box>
            ))
        )
    }

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
                    <PriceListItem
                        name={translate('basePrice', language)}
                        price={basePrice}
                    >
                    </PriceListItem>

                    {sections.map((section, index) => (
                        <Accordion key={index}>
                            <AccordionSummary
                                expandIcon={<ExpandMoreOutlined />}
                                aria-controls="panel1a-content"
                                id="panel1a-header"
                            >
                                <Typography variant="h4">{section.name}</Typography>
                            </AccordionSummary>
                            <AccordionDetails>
                                {renderSectionContent(section)}
                            </AccordionDetails>
                        </Accordion>
                    ))}
                        {/* <Box key={index}>
                            <Typography variant="h4">{section.name}</Typography>
                            {groups.filter(g => section.optionGroupIds.includes(g.id)).map((group, index) => (
                                <Box key={index}>
                                    <Typography vairant="body1">{group.name}</Typography>

                                    {selectedOptions.filter(o => group.optionIds.includes(o)).map((option, index) => (
                                        <PriceListItem
                                            key={index}
                                            optionId={option}
                                        >
                                        </PriceListItem>
                                    ))}
                                </Box>
                            ))}
                        </Box> */}
                    
                    {/* <Typography>{translate('priceList', language)}</Typography>
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
                    ))} */}
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
        sections: selectOptionSections(state),
        groups: selectOptionGroups(state),
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
