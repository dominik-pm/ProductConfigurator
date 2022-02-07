import { Delete } from '@mui/icons-material'
import { Box, Checkbox, FormControl, Grid, IconButton, InputLabel, ListItemText, MenuItem, OutlinedInput, Select, Tooltip, Typography } from '@mui/material'
import React from 'react'
import { connect } from 'react-redux'
import { translate } from '../../../../lang'
import { extractGroupIdFromBuilderOption, extractGroupNameFromBuilderGroup, getBuilderGroupById, getBuilderOptionById, getBuilderOptionIncompatibilitiesByOptionId, getBuilderOptionRequirementsByOptionId, selectBuilderOptions } from '../../../../state/configurationBuilder/builderSelectors'
import { deleteOption, setOptionIncompatibilities, setOptionRequirements } from '../../../../state/configurationBuilder/builderSlice'
import { selectLanguage } from '../../../../state/language/languageSelectors'

function BuilderOption({ optionId, group, option, allOptions, optionReqirements, optionIncompatibilities, language, remove, setOptionRequirements, setOptionIncompatibilities }) {

    const { name, description } = option

    function handleDelete() {
        remove(group.id, optionId)
    }

    const arrayFromMultiSelect = (event) => {
        const {
            target: { value }
        } = event

        // on autofill we get a stringified value
        return typeof value === 'string' ? value.split(',') : value
    }

    function handleSetRequirements(event) {
        const newRequirements = arrayFromMultiSelect(event)

        setOptionRequirements({optionId, requirements: newRequirements})
    }

    function handleSetIncompatibilities(event) {
        const newIncomps = arrayFromMultiSelect(event)

        setOptionIncompatibilities({optionId, incompatibilities: newIncomps})
    }

    return (
        <Grid container paddingTop={2} paddingLeft={2} paddingBottom={2}>
            <Grid item width="100%">
                <Box display="flex" justifyContent="space-between">
                    {/* Info */}
                    <Box>
                        <Typography variant="body1">{name}</Typography>
                        <Typography variant="body2">{description}</Typography>
                    </Box>

                    {/* Actions */}
                    <Tooltip title={`${translate('remove', language)} '${name}'`}>
                        <IconButton onClick={handleDelete}>
                            <Delete />
                        </IconButton>
                    </Tooltip>
                </Box>
            </Grid>

            <Grid item container justifyContent="center">
                {Multiselect(translate('requirements', language), optionReqirements, allOptions.filter(o => o.id !== optionId), handleSetRequirements)}
                {Multiselect(translate('incompatibilities', language), optionIncompatibilities, allOptions.filter(o => o.id !== optionId), handleSetIncompatibilities)}
            </Grid>
        </Grid>
    )
}

const Multiselect = (title, resultOptions, allOptions, onChangeCallback) => (
    <FormControl sx={{ m: 1, width: 300 }}>
        <InputLabel id={`options-label-${title}`}>{title}</InputLabel>
        <Select
            labelId={`options-label-${title}`}
            multiple
            value={resultOptions}
            onChange={onChangeCallback}
            input={<OutlinedInput label={title} />}
            renderValue={(selectedIds) => {
                    const selectedOptions = allOptions.filter(o => selectedIds.includes(o.id))
                    return selectedOptions.map(o => o.name).join(', ')
                }
            }
        >
            {allOptions.map((option) => {
                const groupId = extractGroupIdFromBuilderOption(option)
                const groupName = groupId
                return (
                    <MenuItem key={option.id} value={option.id}>
                        <Checkbox checked={resultOptions.indexOf(option.id) > -1} />
                        <ListItemText primary={`${option.name} (${groupName})`} />
                    </MenuItem>
                )
            })}
        </Select>
    </FormControl>
)

const mapStateToProps = (state, ownProps) => ({
    option: getBuilderOptionById(state, ownProps.optionId),
    allOptions: selectBuilderOptions(state),
    optionReqirements: getBuilderOptionRequirementsByOptionId(state, ownProps.optionId),
    optionIncompatibilities: getBuilderOptionIncompatibilitiesByOptionId(state, ownProps.optionId),
    language: selectLanguage(state)
})
const mapDispatchToProps = {
    remove: deleteOption,
    setOptionRequirements,
    setOptionIncompatibilities
}
export default connect(
    mapStateToProps, 
    mapDispatchToProps
)(BuilderOption)