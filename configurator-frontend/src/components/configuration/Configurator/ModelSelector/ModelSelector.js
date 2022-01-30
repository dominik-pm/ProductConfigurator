import { Grid, Typography } from '@mui/material'
import { Box } from '@mui/system'
import React from 'react'
import { connect } from 'react-redux'
import { selectModels, selectSelectedModel } from '../../../../state/configuration/configurationSelectors'
import ModelButton from './ModelButton'

function ModelSelector({ models, selectedModel }) {
    
    return (
        <Box marginBottom={4}>
            <Typography variant="h3">Models</Typography>

            <Grid container spacing={2} alignItems="flex-start">

                {models.map((model, index) => (
                    <Grid item key={index} xs={12} sm={6} md={4} lg={3}>
                        <ModelButton model={model} isSelected={model.name === selectedModel}></ModelButton>
                    </Grid>
                ))}

                <Grid item xs={12} sm={6} md={4} lg={3}>
                    <ModelButton model={null} isSelected={!selectedModel} disabled={true}></ModelButton>
                </Grid>

            </Grid>
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
