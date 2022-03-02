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
            
            createModel(name, description)

        })
    }

    function handleSetDefaultModel(event) {
        setDefaultModel(event.target.value)
    }

    return (
        <Box width="100%">
            <Grid container justifyContent="space-between">

                <Typography variant="h3">{translate('models', language)}</Typography>

                <Box display="flex">
                    <Tooltip title="Add Model">
                        <IconButton sx={{marginRight: 2}} onClick={handleAddModel}>
                            <Add />
                        </IconButton>
                    </Tooltip>

                    <Box sx={{ minWidth: 120 }}>
                        <FormControl variant='standard' 
                            fullWidth
                            >
                            <InputLabel id="select-default-model-label">{translate('defaultModel', language)}</InputLabel>
                            <Select
                                labelId='select-default-model-label'
                                value={selectedDefaultModel}
                                autoWidth
                                label="Default Model"
                                onChange={handleSetDefaultModel}
                                >
                                {models.map((model, index) => (
                                    <MenuItem key={index} value={model.id}>{model.id}</MenuItem>
                                    ))}
                            </Select>
                        </FormControl>
                    </Box>
                </Box>
            </Grid>

            <Grid container gap={2} marginTop={2} direction="column" justifyContent="center" alignItems="center">

                {models.map((model, index) => (
                    <Model key={index} model={model} isSelected={model.id === selectedDefaultModel}></Model>
                ))}

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