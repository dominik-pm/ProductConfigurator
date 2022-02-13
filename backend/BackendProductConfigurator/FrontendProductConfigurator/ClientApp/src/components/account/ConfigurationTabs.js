import { Box, Tab, Tabs } from '@mui/material'
import React, { useEffect, useState } from 'react'
import { connect } from 'react-redux'
import { useNavigate, useParams } from 'react-router-dom'
import { translate } from '../../lang'
import { selectLanguage } from '../../state/language/languageSelectors'
import { selectAllOrderedConfigurations, selectIsAdmin, selectOrderedConfigurations, selectSavedConfigurations } from '../../state/user/userSelector'
import ConfigurationList from './ConfigurationList'

function TabPanel(props) {
    const { children, value, index, ...other } = props

    return (
        <div
            role="tabpanel"
            hidden={value !== index}
            id={`simple-tabpanel-${index}`}
            aria-labelledby={`simple-tab-${index}`}
            {...other}
        >
            {value === index && (
                <Box sx={{ p: 3 }}>
                    {children}
                </Box>
            )}
        </div>
    )
}

const tabNames = [
    {name: 'saved', value: 0},
    {name: 'ordered', value: 1},
    {name: 'allordered', value: 2, forAdmin: true}
]

function ConfigurationTabs({ isAdmin, savedConfigurations, orderedConfigurations, allOrderedConfigurations, language }) {

    const navigate = useNavigate()
    const { tab } = useParams()

    const foundTab = tabNames.find(t => t.name === tab)
    let tabName = foundTab

    if (foundTab && tabName.forAdmin && !isAdmin) {
        // the tab is only for admins and the user is not an admin
        tabName = null
    }

    useEffect(() => {
        // if the dynamic path tabname does not correspond to any of tabnames array -> update route by navigating
        if (!tabName) {
            navigate(`/account/${tabNames[0].name}`)
        }
    }, [tabName, navigate])

    const tabIndex = tabName ? tabName.value : 0

    const [value, setValue] = useState(tabIndex)

    const handleChange = (event, newValue) => {
        const tabName = tabNames.find(t => t.value === newValue)
        if (tabName) {
            setValue(tabName.value)
            navigate(`/account/${tabName.name}`)
        }
    }

    function tabProps(index) {
        return {
            id: `simple-tab-${index}`,
            'aria-controls': `simple-tabpanel-${index}`,
        }
    }

    return (
        <Box width="100%">
            <Tabs 
                scrollButtons="auto"
                variant="scrollable" 
                allowScrollButtonsMobile
                value={value}
                onChange={handleChange} 
                aria-label="configurationstabs"
            >
                <Tab label={translate('savedConfigurations', language)} {...tabProps(0)} />
                <Tab label={translate('orderedConfigurations', language)} {...tabProps(1)} />
                {isAdmin ? <Tab label={translate('allOrderedConfigurations', language)} {...tabProps(2)} /> : ''}
            </Tabs>
            <TabPanel value={value} path={tabNames[0].name} index={0}>
                <ConfigurationList configurations={savedConfigurations}></ConfigurationList>
            </TabPanel>
            <TabPanel value={value} path={tabNames[1].name} index={1}>
                <ConfigurationList configurations={orderedConfigurations} isOrdered={true}></ConfigurationList>
            </TabPanel>
            {isAdmin ?
                <TabPanel value={value} path={tabNames[1].name} index={2}>
                    <ConfigurationList configurations={allOrderedConfigurations} isOrdered={true} isAdminView={true}></ConfigurationList>
                </TabPanel>
                : ''
            }
        </Box>
    )
}

const mapStateToProps = (state) => ({
    isAdmin: selectIsAdmin(state),
    savedConfigurations: selectSavedConfigurations(state),
    orderedConfigurations: selectOrderedConfigurations(state),
    allOrderedConfigurations: selectAllOrderedConfigurations(state),
    language: selectLanguage(state)
})
const mapDispatchToProps = {
    
}
export default connect(
    mapStateToProps, 
    mapDispatchToProps
)(ConfigurationTabs)