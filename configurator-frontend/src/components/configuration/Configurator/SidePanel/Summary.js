import React, { useEffect } from 'react'
import { connect } from 'react-redux'
import { Box } from '@mui/system'
import { Accordion, AccordionDetails, AccordionSummary, Divider, List, ListItem, ListItemText, ListSubheader, Stack, Typography } from '@mui/material'
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
            console.log('summary: fetching configuration ', configurationId)
            fetchConfiguration(configurationId)
        }
    }, [configurationId, loadedConfigurationId, fetchConfiguration])


    function renderSectionContent(section) {
        let sectionOptions = []
        const sectionGroups = groups.filter(g => section.optionGroupIds.includes(g.id))
        sectionGroups.forEach(g => {
            const groupOptions = selectedOptions.filter(o => g.optionIds.includes(o))
            sectionOptions.push(...groupOptions)
        })

        return (
            sectionOptions.map(optionId => (
                <ListItem sx={{paddingBottom: 0, paddingTop: 0}} key={optionId}>
                    <PriceListItem
                        key={optionId}
                        optionId={optionId}
                    >
                    </PriceListItem>
                </ListItem>
            ))
        )

        // const optionGroups = groups.filter(g => section.optionGroupIds.includes(g.id))
        // const groupOptions = (group) => selectedOptions.filter(o => group.optionIds.includes(o))

        // return (
        //     optionGroups.map((group, index) => (
        //         <Box key={index}>
        //             {groupOptions(group).length > 0 ?
        //                 <>
        //                     <Typography vairant="body1">{group.name}</Typography>
                            
        //                     {groupOptions(group).map((option, index) => (
        //                         <PriceListItem
        //                         key={index}
        //                         optionId={option}
        //                         >
        //                         </PriceListItem>
        //                     ))}
        
        //                     <Divider sx={{marginTop: 1, marginBottom: 1}} />

        //                 </> : ''
        //             }
        //         </Box>
        //     ))
        // )
    }

    function renderSummary() {
        if (status !== 'succeeded') {
            return (<></>)
        }

        return (
            <Box display="flex" alignItems="center" flexDirection="column">
                <Typography variant="h3">
                    {translate('price', language)}: {currentPrice}€
                </Typography>

                <Box width="100%">
                    <List
                        dense={true}
                        sx={{
                            position: 'relative',
                            overflow: 'auto',
                            maxHeight: 300,
                            '& ul': { padding: 0 },
                        }}
                        subheader={<li />}
                    >
                        <li>
                            <ul>
                                <ListSubheader sx={{textAlign: 'center'}}>{translate('basePrice', language)}</ListSubheader>

                                <ListItem sx={{paddingBottom: 0, paddingTop: 0}}>
                                    <PriceListItem
                                        name={translate('basePrice', language)}
                                        price={basePrice}
                                        >
                                    </PriceListItem>
                                </ListItem>
                            </ul>
                        </li>
                        <Divider sx={{marginX: 4, marginTop: 2}} />

                        {sections.map(section => (
                            <li key={section.id}>
                                <ul>
                                    <ListSubheader sx={{textAlign: 'center'}}>{section.name}</ListSubheader>

                                    {renderSectionContent(section)}
                                </ul>
                                <Divider sx={{marginX: 4, marginTop: 2}} />
                            </li>
                        ))}
                    </List>
                </Box>
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
