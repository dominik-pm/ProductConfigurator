import React, { useState } from 'react'

import OptionGroup from './Options/OptionGroup'
import PropTypes from 'prop-types'
import Tabs from '@mui/material/Tabs'
import Tab from '@mui/material/Tab'
import Box from '@mui/material/Box'
import { useSelector } from 'react-redux'

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
TabPanel.propTypes = {
    children: PropTypes.node,
    index: PropTypes.number.isRequired,
    value: PropTypes.number.isRequired
}

export default function OptionTabs() {
    // const dispatch = useDispatch()
    const {configuration} = useSelector(state => state.configuration)

    const [value, setValue] = useState(0)

    const handleChange = (event, newValue) => {
        setValue(newValue)
    }

    function render() {
        return (
            <Box sx={{ width: '100%' }}>
                <Box sx={{ borderBottom: 1, borderColor: 'divider' }}>
                    <Tabs value={value} onChange={handleChange} aria-label="sectiontabs">
                        {/* <Tab label="Item One" {...a11yProps(0)} />
                        <Tab label="Item Two" {...a11yProps(1)} />
                        <Tab label="Item Three" {...a11yProps(2)} /> */}
                        {configuration.optionSections.map((section, index) => (
                            <Tab key={section.id} label={section.name} {...a11yProps(index)} />
                        ))}
                    </Tabs>
                </Box>
                {configuration.optionSections.map((section, index) => (
                    <TabPanel key={section.id} value={value} index={index}>
                        {configuration.optionGroups
                            .filter(group => section.optionGroupIds.includes(group.id))
                            .map((group, index) => (
                                <OptionGroup key={group.id} group={group}></OptionGroup>
                            ))}
                    </TabPanel>
                ))}
                {/* <TabPanel value={value} index={0}>
                    Item One 111
                </TabPanel>
                <TabPanel value={value} index={1}>
                    Item Two
                </TabPanel>
                <TabPanel value={value} index={2}>
                    Item Three
                </TabPanel> */}
            </Box>
        )
    }

    return (
        render()
    )
}


function a11yProps(index) {
    return {
        id: `simple-tab-${index}`,
        'aria-controls': `simple-tabpanel-${index}`,
    }
}