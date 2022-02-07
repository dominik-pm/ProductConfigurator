import React from 'react'
import { connect } from 'react-redux'
import { Checkbox, FormControl, Grid, IconButton, InputLabel, ListItemText, MenuItem, OutlinedInput, Select, Tooltip, Typography } from '@mui/material'
import { changeModelOptions, removeModel } from '../../../../state/configurationBuilder/builderSlice'
import { Delete } from '@mui/icons-material'
import { confirmDialogOpen } from '../../../../state/confirmationDialog/confirmationSlice'
import { translate } from '../../../../lang'
import { selectLanguage } from '../../../../state/language/languageSelectors'
import { extractGroupIdFromBuilderOption, selectBuilderOptions } from '../../../../state/configurationBuilder/builderSelectors'

function Model({ model, isSelected = false, allOptions, removeModel, setModelOptions, openConfirmDialog, language }) {

    const { name, description, options } = model

    function handleDelete() {
        openConfirmDialog(`${translate('removeModelConfirmation', language)} '${name}'?`, {}, null, () => {
            removeModel(name)
        })
    }

    function handleChangeOptions(event) {
        const {
            target: { value },
        } = event

        // On autofill we get a stringified value. 
        const newOptions = typeof value === 'string' ? value.split(',') : value

        setModelOptions(name, newOptions)
    }

    return (
        <Grid container>
            {/* Info */}
            <Grid 
                item xs={12} sm={10} xl={9}
            >
                <Typography variant="body1">
                    {name}
                </Typography>
                <Typography variant="body2">
                    {description}
                </Typography>
            </Grid>

            {/* Actions */}
            <Grid 
                item container xs={12} sm={2} xl={1} 
                justifyContent={{xs: 'center', sm: 'flex-end'}}
            >
                

                <Tooltip title={`${translate('remove', language)} '${name}'`}>
                    <IconButton onClick={handleDelete}>
                        <Delete />
                    </IconButton>
                </Tooltip>
            </Grid>

            {/* Option Select */}
            <Grid
                item container xs={12} xl={2}
                justifyContent={{xs: 'center', xl: 'flex-end'}}
            >
                <FormControl sx={{ m: 1, width: 300 }}>
                    <InputLabel id={`options-label-${name}`}>{translate('options', language)}</InputLabel>
                    <Select
                        labelId="options-label"
                        multiple
                        value={options}
                        onChange={handleChangeOptions}
                        input={<OutlinedInput label={translate('options', language)} />}
                        renderValue={(selectedIds) => {
                                const selectedOptions = allOptions.filter(o => selectedIds.includes(o.id))
                                return selectedOptions.map(o => o.name).join(', ')
                            }
                        }
                    >
                        {allOptions.map((option) => (
                            <MenuItem key={option.id} value={option.id}>
                                <Checkbox checked={options.indexOf(option.id) > -1} />
                                <ListItemText primary={`${option.name} (${extractGroupIdFromBuilderOption(option)})`} />
                            </MenuItem>
                        ))}
                    </Select>
                </FormControl>
            </Grid>
            
        </Grid>
    )
}

const mapStateToProps = (state) => ({
    allOptions: selectBuilderOptions(state),
    language: selectLanguage(state)
})
const mapDispatchToProps = {
    removeModel: removeModel,
    setModelOptions: changeModelOptions,
    openConfirmDialog: confirmDialogOpen
}
export default connect(
    mapStateToProps, 
    mapDispatchToProps
)(Model)