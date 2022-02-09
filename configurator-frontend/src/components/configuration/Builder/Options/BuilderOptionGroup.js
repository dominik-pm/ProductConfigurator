import { Checkbox, Divider, FormControl, FormControlLabel, Grid, IconButton, InputLabel, ListItemText, MenuItem, OutlinedInput, Select, Tooltip, Typography } from '@mui/material'
import { Box } from '@mui/system'
import React from 'react'
import { selectLanguage } from '../../../../state/language/languageSelectors'
import { translate } from '../../../../lang'
import { connect } from 'react-redux'
import BuilderOption from './BuilderOption'
import { Add, Delete } from '@mui/icons-material'
import { alertTypes, openAlert } from '../../../../state/alert/alertSlice'
import { createOption, deleteOptionGroup, setGroupIsReplacement, setGroupIsRequired, setGroupRequirements } from '../../../../state/configurationBuilder/builderSlice'
import { inputDialogOpen } from '../../../../state/inputDialog/inputDialogSlice'
import { confirmDialogOpen } from '../../../../state/confirmationDialog/confirmationSlice'
import { getBuilderGroupRequirementsByGroupId, selectBuilderGroups } from '../../../../state/configurationBuilder/builderSelectors'

function OptionGroup({group, sectionId, allGroups, groupRequirements, createOption, setGroupRequirements, setGroupIsRequired, setGroupIsReplacement, deleteGroup, openInputDialog, openConfirmDialog, openAlert, language}) {

    const { id, name, description, optionIds, required, replacement } = group

    function handleAddOption() {
        const data = {
            optionName: {name: 'name', value: '' },
            optionDescription: {name: 'description', value: ''},
        }
        openInputDialog(translate('newOption', language), data, (data) => {
            const success = createOption(id, data.optionName.value, data.optionDescription.value)
            if (!success) {
                openAlert('Option already exists!', alertTypes.ERROR)
            }
        })
    }

    function handleRemoveGroup() {
        openConfirmDialog(`${translate('removeGroupConfirmation', language)}: ${name}?`, {}, null, () => {
            deleteGroup(id, sectionId)
        })
    }

    function handleSetGroupRequirements(event) {
        const {
            target: { value }
        } = event

        // on autofill we get a stringified value
        const requirements = typeof value === 'string' ? value.split(',') : value
        setGroupRequirements({groupId: id, requirements: requirements})
    }

    function handleChangeRequired(event) {
        setGroupIsRequired({groupId: id, required: !required})
    }

    function handleChangeMultiselect(event) {
        setGroupIsReplacement({groupId: id, replacement: !replacement})
    }

    return (
        <Box marginBottom={1} padding={1} sx={{border: '1px dashed grey'}}>
            <Box display={'flex'} flexWrap="wrap" justifyContent={'space-between'} alignItems={'center'}>
                <Box>
                    <Typography variant="h3">
                        {name}
                    </Typography>
                    <Typography variant="subtitle1">{description}</Typography>
                </Box>

                <Box>
                    <Tooltip title={translate('addOption', language)}>
                        <IconButton onClick={handleAddOption}>
                            <Add />
                        </IconButton>
                    </Tooltip>
                    <Tooltip title={`${translate('remove', language)} '${name}'`}>
                        <IconButton onClick={handleRemoveGroup}>
                            <Delete />
                        </IconButton>
                    </Tooltip>
                </Box>
            </Box>

            <Grid item container justifyContent="center">
                {Multiselect(translate('requirements', language), name, groupRequirements, allGroups.filter(g => g.id !== id), handleSetGroupRequirements)}
                {CheckboxInput(translate('required', language), required, (e) => handleChangeRequired(e))}
                {CheckboxInput(translate('multiselect', language), !replacement, (e) => handleChangeMultiselect(e))}
            </Grid>
            
            <Box>
                {optionIds.map(optionId => (
                    <div key={optionId}>
                        <Divider variant="middle" />
                        <BuilderOption key={optionId} group={group} optionId={optionId}></BuilderOption>
                    </div>
                ))}
            </Box>
        </Box>
    )
}

const CheckboxInput = (title, checked, callback) => {
    return (
        <FormControl sx={{ m: 1, width: 300 }}>
            <FormControlLabel
                label={title}
                labelPlacement="end"
                control={
                    <Checkbox
                        checked={checked}
                        onChange={(event) => {
                            callback(event)
                        }}
                    />
                }
            />
        </FormControl>
    )
}

const Multiselect = (title, groupName, resultGroups, allGroups, onChangeCallback) => (
    <FormControl sx={{ m: 1, width: 300 }}>
        <InputLabel id={`${title}-${groupName}`}>{title}</InputLabel>
        <Select
            labelId={`${title}-${groupName}`}
            multiple
            value={resultGroups}
            onChange={onChangeCallback}
            input={<OutlinedInput label={title} />}
            renderValue={(selectedIds) => {
                    const selectedGroups = allGroups.filter(g => selectedIds.includes(g.id))
                    return selectedGroups.map(g => g.name).join(', ')
                }
            }
        >
            {allGroups.map((group) => (
                <MenuItem key={group.id} value={group.id}>
                    <Checkbox checked={resultGroups.indexOf(group.id) > -1} />
                    <ListItemText primary={group.name} />
                </MenuItem>
            ))}
        </Select>
    </FormControl>
)


const mapStateToProps = (state, ownProps) => ({
    allGroups: selectBuilderGroups(state),
    groupRequirements: getBuilderGroupRequirementsByGroupId(state, ownProps.group.id),
    language: selectLanguage(state)
})
const mapDispatchToProps = {
    createOption,
    setGroupRequirements,
    setGroupIsRequired,
    setGroupIsReplacement,
    deleteGroup: deleteOptionGroup,
    openInputDialog: inputDialogOpen,
    openConfirmDialog: confirmDialogOpen,
    openAlert
}
export default connect(
    mapStateToProps,
    mapDispatchToProps
)(OptionGroup)
