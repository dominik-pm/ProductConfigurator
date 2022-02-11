import React, { useState } from 'react'

import PropTypes from 'prop-types'
import Tabs from '@mui/material/Tabs'
import Tab from '@mui/material/Tab'
import Box from '@mui/material/Box'
import { Button, IconButton, Stack, Tooltip, Typography } from '@mui/material'
import { Add } from '@mui/icons-material'
import { connect } from 'react-redux'
import { changeSectionProperties, createGroup, createSection, deleteSection } from '../../../state/configurationBuilder/builderSlice'
import { inputDialogOpen } from '../../../state/inputDialog/inputDialogSlice'
import { selectLanguage } from '../../../state/language/languageSelectors'
import { alertTypes, openAlert } from '../../../state/alert/alertSlice'
import { getBuilderGroupsInSection, selectBuilderGroups, selectBuilderSectionsFromCurrentLanguage } from '../../../state/configurationBuilder/builderSelectors'
import BuilderOptionGroup from './Options/BuilderOptionGroup'
import { translate } from '../../../lang'
import { confirmDialogOpen } from '../../../state/confirmationDialog/confirmationSlice'
import EditButton from './EditButton'

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

// a tab has to have a children prop
function TooltipAsTab({ children, title }) {
    return <Tooltip title={title} children={children} />
}

function SectionTabs({ sections, getOptionGroupsInSection, optionGroups, openInputDialog, openConfirmDialog, createSection, createGroup, changeSectionProperties, deleteSection, openAlert, language }) {

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

    const handleRemoveSection = (sectionId, name) => {
        openConfirmDialog(`${translate('removeSectionConfirmation', language)}: ${name}?`, {}, null, () => {
            deleteSection(sectionId)
            setValue(0)
        })
    }

    const handleAddGroup = (sectionId) => {
        const data = {
            groupName: {name: 'name', value: '' },
            groupDescription: {name: 'description', value: ''},
            groupIsRequired: {name: 'is required', value: false, isCheckBox: true},
            groupIsMultiselect: {name: 'multiselect', value: true, isCheckBox: true}
        }
        openInputDialog(translate('newGroup', language), data, (data) => {
            const isReplacementGroup = !data.groupIsMultiselect.value   // is replacement group if its not a multiselect
            const success = createGroup(sectionId, data.groupName.value, data.groupDescription.value, data.groupIsRequired.value, isReplacementGroup)
            if (!success) {
                openAlert('Group already exists!', alertTypes.ERROR)
            }
        })
    }

    const renderSectionHeader = (sectionId, sectionName) => {
        return (
            <Box>
                <Button onClick={() => handleAddGroup(sectionId)}>{translate('addOptionGroup', language)}</Button>
                <EditButton 
                    title={`${translate('editSectionName', language)}`}
                    propertyName={translate('sectionName', language)}
                    oldValue={sectionName}
                    valueChangedCallback={(newValue) => {changeSectionProperties({sectionId, newName: newValue})}}
                    textButton={true}
                ></EditButton>
                <Button onClick={() => handleRemoveSection(sectionId, sectionName)}>{translate('removeSection', language)}</Button>
            </Box>
        )
    }

    return (
        <Box sx={{ width: '100%' }}>
            <Typography variant="h3">{translate('options', language)}</Typography>

            <Box sx={{ borderBottom: 1, borderColor: 'divider' }}>
                <Tabs 
                    value={value} 
                    scrollButtons="auto"
                    allowScrollButtonsMobile
                    variant="scrollable" 
                    onChange={handleChange} 
                    aria-label="sectiontabs"
                >
                    {sections.map((section, index) => (
                        <Tab key={section.id} label={section.name} wrapped {...a11yProps(index)} />
                    ))}

                    <TooltipAsTab title={translate('addSection', language)}>
                        <IconButton onClick={() => handleAddSection()}>
                            <Add />
                        </IconButton>
                    </TooltipAsTab>
                </Tabs>
            </Box>
            
            {sections.map((section, index) => (
                <TabPanel key={section.id} value={value} index={index}>
                    <Stack minHeight={400} mb={10}>
                        {renderSectionHeader(section.id, section.name)}

                        {optionGroups
                            .filter(group => getOptionGroupsInSection(section.id).includes(group.id))
                            .map((group, index) => (
                                <BuilderOptionGroup key={group.id} group={group} sectionId={section.id}></BuilderOptionGroup>
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
    sections: selectBuilderSectionsFromCurrentLanguage(state),
    getOptionGroupsInSection: (sectionId) => getBuilderGroupsInSection(state, sectionId),
    optionGroups: selectBuilderGroups(state),
    language: selectLanguage(state),
})
const mapDispatchToProps = {
    openInputDialog: inputDialogOpen,
    openConfirmDialog: confirmDialogOpen,
    createSection,
    changeSectionProperties,
    deleteSection,
    createGroup,
    openAlert
}
export default connect(
    mapStateToProps, 
    mapDispatchToProps
)(SectionTabs)