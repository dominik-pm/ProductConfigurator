import { Delete } from '@mui/icons-material'
import { Box, Checkbox, FormControl, Grid, IconButton, InputAdornment, InputLabel, ListItemText, MenuItem, OutlinedInput, Select, TextField, Tooltip, Typography } from '@mui/material'
import React from 'react'
import { useState } from 'react'
import { connect } from 'react-redux'
import { translate } from '../../../../lang'
import { extractGroupIdFromBuilderOption, extractGroupNameFromBuilderGroup, getBuilderGroupById, getBuilderOptionById, getBuilderOptionIncompatibilitiesByOptionId, getBuilderOptionPrice, getBuilderOptionRequirementsByOptionId, selectBuilderOptions } from '../../../../state/configurationBuilder/builderSelectors'
import { changeOptionProperties, deleteOption, setOptionIncompatibilities, setOptionPrice, setOptionRequirements } from '../../../../state/configurationBuilder/builderSlice'
import { selectLanguage } from '../../../../state/language/languageSelectors'
import EditButton from '../EditButton'

function BuilderOption({ optionId, group, option, optionPrice, allOptions, optionReqirements, optionIncompatibilities, language, remove, setOptionPrice, setOptionRequirements, setOptionIncompatibilities, changeOptionProperties }) {

    const { name, description } = option

    const [priceError, setPriceError] = useState(false)

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

    function handlePriceChanged(event) {
        const price = Number(event.target.value)

        // check if the price is valid
        if (price < 0) {
            setPriceError(true)
            return
        }

        setPriceError(false)
        setOptionPrice({optionId, price})
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
                        <Typography variant="body1">
                            {name}
                            <EditButton 
                                title={`${translate('edit', language)}`} 
                                propertyName={translate('optionName', language)} 
                                oldValue={name} 
                                valueChangedCallback={(newValue) => {changeOptionProperties({optionId, newName: newValue})}}
                            ></EditButton>
                        </Typography>
                        <Typography variant="body2">
                            {description}
                            <EditButton 
                                title={`${translate('edit', language)}`} 
                                propertyName={translate('optionDescription', language)} 
                                oldValue={description} 
                                valueChangedCallback={(newValue) => {changeOptionProperties({optionId, newDescription: newValue})}}
                            ></EditButton>
                        </Typography>
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
                <FormControl sx={{ m: 1, width: 300 }}>
                    <TextField 
                        fullWidth
                        label={translate('optionPrice', language)}
                        variant="outlined"
                        onChange={handlePriceChanged}
                        error={priceError}
                        defaultValue={optionPrice}
                        type="number"
                        InputProps={{
                            inputProps: {
                                min: 0
                            },
                            startAdornment: <InputAdornment position='start'>€</InputAdornment>
                        }}
                    />
                </FormControl>
                <FormControl sx={{ m: 1, width: 300 }}>
                    {Multiselect(translate('requirements', language), optionReqirements, allOptions.filter(o => o.id !== optionId), handleSetRequirements)}
                </FormControl>
                <FormControl sx={{ m: 1, width: 300 }}>
                    {Multiselect(translate('incompatibilities', language), optionIncompatibilities, allOptions.filter(o => o.id !== optionId), handleSetIncompatibilities)}
                </FormControl>
            </Grid>
        </Grid>
    )
}

const Multiselect = (title, resultOptions, allOptions, onChangeCallback) => (
    <>
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
    </>
)

const mapStateToProps = (state, ownProps) => ({
    option: getBuilderOptionById(state, ownProps.optionId),
    optionPrice: getBuilderOptionPrice(state, ownProps.optionId),
    allOptions: selectBuilderOptions(state),
    optionReqirements: getBuilderOptionRequirementsByOptionId(state, ownProps.optionId),
    optionIncompatibilities: getBuilderOptionIncompatibilitiesByOptionId(state, ownProps.optionId),
    language: selectLanguage(state)
})
const mapDispatchToProps = {
    remove: deleteOption,
    setOptionRequirements,
    setOptionIncompatibilities,
    setOptionPrice,
    changeOptionProperties
}
export default connect(
    mapStateToProps, 
    mapDispatchToProps
)(BuilderOption)