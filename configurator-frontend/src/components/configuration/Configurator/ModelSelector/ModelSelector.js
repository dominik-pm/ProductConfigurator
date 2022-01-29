import { Typography } from '@mui/material'
import { Box } from '@mui/system'
import React from 'react'
import { connect } from 'react-redux'
import { selectModels, selectSelectedModel } from '../../../../state/configuration/configurationSelectors'
import ModelButton from './ModelButton'

function ModelSelector({ models, selectedModel }) {
    return (
        <Box marginBottom={4}>
            <Typography variant="h3">Models</Typography>
            <Box display="flex" justifyContent="space-evenly" alignItems="center" flexWrap="wrap">

                {models.map((model, index) => (
                    <ModelButton key={index} model={model} isSelected={model.modelName === selectedModel}></ModelButton>
                ))}

                <ModelButton model={{modelName: 'custom', description: ''}} isSelected={!selectedModel} disabled={true}></ModelButton>

            </Box>
        </Box>
    )
}

const mapStateToProps = (state) => ({
    models: selectModels(state),
    selectedModel: selectSelectedModel(state),
})
const mapDispatchToProps = {

}
export default connect(
    mapStateToProps, 
    mapDispatchToProps
)(ModelSelector)
