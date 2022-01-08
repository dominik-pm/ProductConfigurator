import React, { useState } from 'react'

import PropTypes from 'prop-types'
import Tabs from '@mui/material/Tabs'
import Tab from '@mui/material/Tab'
import Box from '@mui/material/Box'
import { IconButton, Typography } from '@mui/material'
import { Add } from '@mui/icons-material'
import { connect } from 'react-redux'

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

function a11yProps(index) {
    return {
        id: `simple-tab-${index}`,
        'aria-controls': `simple-tabpanel-${index}`,
    }
}

function SectionTabs({configuration}) {

    const [value, setValue] = useState(0)

    const handleChange = (event, newValue) => {
        setValue(newValue)
    }


    return (
        <Box sx={{ width: '100%' }}>
            <Box sx={{ borderBottom: 1, borderColor: 'divider' }}>
                <Tabs 
                    value={value} 
                    scrollButtons="auto"
                    allowScrollButtonsMobile
                    variant="scrollable" 
                    onChange={handleChange} 
                    aria-label="sectiontabs"
                >
                    <Tab label="Item One" {...a11yProps(0)} />
                    <Tab label="Item Two" {...a11yProps(1)} />
                    <Tab label="Item Three" {...a11yProps(2)} />

                    {/* {configuration.optionSections.map((section, index) => (
                        <Tab key={section.id} label={section.name} wrapped {...a11yProps(index)} />
                    ))} */}

                    <IconButton onClick={() => console.log('ADD')}>
                        <Add />
                    </IconButton>
                </Tabs>
            </Box>
            {/* {configuration.optionSections.map((section, index) => (
                <TabPanel key={section.id} value={value} index={index}>
                    
                </TabPanel>
            ))} */}
            <TabPanel value={value} index={0}>
                <Typography variant="body1">Item One 1</Typography>
            </TabPanel>
            <TabPanel value={value} index={1}>
                <Typography variant="body1">Item Two</Typography>
            </TabPanel>
            <TabPanel value={value} index={2}>
                <Typography variant="body1">Item Three</Typography>
            </TabPanel>
        </Box>
    )
}

const mapStateToProps = (state) => ({
    
})
const mapDispatchToProps = {
    
}
export default connect(
    mapStateToProps, 
    mapDispatchToProps
)(SectionTabs)