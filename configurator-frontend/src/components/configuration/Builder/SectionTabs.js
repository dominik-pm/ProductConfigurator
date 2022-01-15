import React, { useState } from 'react'

import PropTypes from 'prop-types'
import Tabs from '@mui/material/Tabs'
import Tab from '@mui/material/Tab'
import Box from '@mui/material/Box'
import { Button, IconButton, Stack, Typography } from '@mui/material'
import { Add } from '@mui/icons-material'
import { connect } from 'react-redux'
import { createGroup, createSection } from '../../../state/configurationBuilder/builderSlice'
import { inputDialogOpen } from '../../../state/inputDialog/inputDialogSlice'
import { selectLanguage } from '../../../state/language/languageSelectors'
import { alertTypes, openAlert } from '../../../state/alert/alertSlice'
import { selectBuilderGroups, selectBuilderSections } from '../../../state/configurationBuilder/builderSelectors'
import BuilderOptionGroup from './Options/BuilderOptionGroup'

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

function SectionTabs({ sections, optionGroups, openInputDialog, createSection, createGroup, openAlert, language }) {

    const [value, setValue] = useState(0)

    const handleChange = (event, newValue) => {
        setValue(newValue)
    }

    const handleAddSection = () => {
        const data = {
            sectionName: {name: 'name', value: '' }
        }
        openInputDialog('New Section', data, (data) => {
            const success = createSection(data.sectionName.value)
            if (!success) {
                openAlert('Section already exists!', alertTypes.ERROR)
            }
        })
    }


    function renderSectionHeader(sectionId) {
        return (
            <Box>
                <Button onClick={() => handleAddGroup(sectionId)}>Add Option Group</Button>
            </Box>
        )
    }
    function handleAddGroup(sectionId) {
        const data = {
            groupName: {name: 'name', value: '' },
            groupDescription: {name: 'description', value: ''},
            groupIsRequired: {name: 'is required', value: false, isCheckBox: true}
        }
        openInputDialog('New Group', data, (data) => {
            const success = createGroup(sectionId, data.groupName.value, data.groupDescription.value, data.groupIsRequired.value)
            if (!success) {
                openAlert('Group already exists!', alertTypes.ERROR)
            }
        })
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
                    {/* <Tab label="Item One" {...a11yProps(0)} />
                    <Tab label="Item Two" {...a11yProps(1)} />
                    <Tab label="Item Three" {...a11yProps(2)} /> */}

                    {sections.map((section, index) => (
                        <Tab key={section.id} label={section.name} wrapped {...a11yProps(index)} />
                    ))}

                    <IconButton onClick={() => handleAddSection()}>
                        <Add />
                    </IconButton>
                </Tabs>
            </Box>
            
            {sections.map((section, index) => (
                <TabPanel key={section.id} value={value} index={index}>
                    <Stack>
                        {renderSectionHeader(section.id)}

                        {optionGroups
                            .filter(group => section.optionGroupIds.includes(group.id))
                            .map((group, index) => (
                                <BuilderOptionGroup key={group.id} group={group}></BuilderOptionGroup>
                        ))}
                    </Stack>
                </TabPanel>
            ))}

            {/* <TabPanel value={value} index={0}>
                <Typography variant="body1">Item One 1</Typography>
            </TabPanel>
            <TabPanel value={value} index={1}>
                <Typography variant="body1">Item Two</Typography>
            </TabPanel>
            <TabPanel value={value} index={2}>
                <Typography variant="body1">Item Three</Typography>
            </TabPanel> */}
        </Box>
    )
}

const mapStateToProps = (state) => ({
    language: selectLanguage(state),
    sections: selectBuilderSections(state),
    optionGroups: selectBuilderGroups(state)
})
const mapDispatchToProps = {
    openInputDialog: inputDialogOpen,
    createSection,
    createGroup,
    openAlert
}
export default connect(
    mapStateToProps, 
    mapDispatchToProps
)(SectionTabs)