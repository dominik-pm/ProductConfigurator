import { Add } from '@mui/icons-material'
import { FormControl, Grid, IconButton, InputLabel, MenuItem, Select, Tooltip, Typography } from '@mui/material'
import { Box } from '@mui/system'
import React from 'react'
import { connect } from 'react-redux'
import { translate } from '../../../../lang'
import { selectBuilderDefaultModel, selectBuilderModels } from '../../../../state/configurationBuilder/builderSelectors'
import { createDefaultModel, createModel } from '../../../../state/configurationBuilder/builderSlice'
import { inputDialogOpen } from '../../../../state/inputDialog/inputDialogSlice'
import { selectLanguage } from '../../../../state/language/languageSelectors'
import Model from './Model'

function ModelSelector({ models, selectedDefaultModel, setDefaultModel, createModel, openInputDialog, language }) {

    function handleAddModel() {
        const title = translate(`addModel`, language)

        const data = {
            modelName: {name: translate('modelName', language), value: '' },
            modelDesc: {name: translate('modelDescription', language), value: '' },
        }

        openInputDialog(title, data, (data) => {
    
            const name = data.modelName.value
            const description = data.modelDesc.value
            
            createModel(name, [], description)

        })
    }

    function handleSetDefaultModel(event) {
        setDefaultModel(event.target.value)
    }

    return (
        <Box width="100%">
            <Grid container justifyContent="space-between">

                <Typography variant="h3">Models</Typography>

                <Box sx={{ minWidth: 120 }}>
                    <FormControl variant='standard' fullWidth>
                        <InputLabel id="select-lang-label">Default Model</InputLabel>
                        <Select
                            labelId='select-lang-label'
                            value={selectedDefaultModel}
                            autoWidth
                            label="Default Model"
                            onChange={handleSetDefaultModel}
                            >
                            {models.map((model, index) => (
                                <MenuItem key={index} value={model.modelName}>{model.modelName}</MenuItem>
                            ))}
                        </Select>
                    </FormControl>
                </Box>
            </Grid>

            <Grid container gap={2} marginTop={2} direction="column" justifyContent="center" alignItems="center">

                {models.map((model, index) => (
                    <Model key={index} model={model} isSelected={model.modelName === selectedDefaultModel}></Model>
                ))}

                <Box>
                    <Tooltip title="Add Model">
                        <IconButton onClick={handleAddModel}>
                            <Add />
                        </IconButton>
                    </Tooltip>
                </Box>

            </Grid>
        </Box>
    )
}

const mapStateToProps = (state) => ({
    models: selectBuilderModels(state),
    selectedDefaultModel: selectBuilderDefaultModel(state),
    language: selectLanguage(state)
})
const mapDispatchToProps = {
    createModel: createModel,
    setDefaultModel: createDefaultModel,
    openInputDialog: inputDialogOpen
}
export default connect(
    mapStateToProps, 
    mapDispatchToProps
)(ModelSelector)
