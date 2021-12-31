import { Box, Tab, Tabs } from '@mui/material'
import React, { useState } from 'react'
import { connect } from 'react-redux'
import { selectAllOrderedConfigurations, selectIsAdmin, selectOrderedConfigurations, selectSavedConfigurations } from '../../state/user/userSelector';
import ConfigurationList from './ConfigurationList';

function TabPanel(props) {
    const { children, value, index, ...other } = props;

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

function ConfigurationTabs({ isAdmin, savedConfigurations, orderedConfigurations, allOrderedConfigurations }) {

    const [value, setValue] = useState(0)

    const handleChange = (event, newValue) => {
        setValue(newValue)
    }

    function tabProps(index) {
        return {
            id: `simple-tab-${index}`,
            'aria-controls': `simple-tabpanel-${index}`,
        }
    }

    return (
        <Box>
            <Tabs value={value} onChange={handleChange} aria-label="configurationstabs">
                <Tab label="Saved Configurations" {...tabProps(0)} />
                <Tab label="Ordered Configurations" {...tabProps(1)} />
                {isAdmin ? <Tab label="All Ordered Configurations" {...tabProps(2)} /> : ''}
            </Tabs>
            <TabPanel value={value} index={0}>
                <ConfigurationList configurations={savedConfigurations}></ConfigurationList>
            </TabPanel>
            <TabPanel value={value} index={1}>
                <ConfigurationList configurations={orderedConfigurations} isOrdered={true}></ConfigurationList>
            </TabPanel>
            {isAdmin ?
                <TabPanel value={value} index={2}>
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
    allOrderedConfigurations: selectAllOrderedConfigurations(state)
})
const mapDispatchToProps = {
    
}
export default connect(
    mapStateToProps, 
    mapDispatchToProps
)(ConfigurationTabs)