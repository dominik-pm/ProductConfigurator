import { Grid, Stack, Typography, useMediaQuery } from '@mui/material'
import { Box } from '@mui/system'
import React from 'react'
import { connect } from 'react-redux'
import { selectModels, selectSelectedModel } from '../../../../state/configuration/configurationSelectors'
import ModelButton from './ModelButton'

function ModelSelector({ models, selectedModel }) {

    const isDesktop = useMediaQuery((theme) => theme.breakpoints.up('sm'))

    function modelSelectGridLayout() {
        return (
            <Grid container spacing={2} alignItems="flex-start">

                {models.map((model, index) => (
                    <Grid item key={index} xs={12} sm={6} md={4} lg={3}>
                        <ModelButton model={model} isSelected={model.id === selectedModel}></ModelButton>
                    </Grid>
                ))}

                <Grid item xs={12} sm={6} md={4} lg={3}>
                    <ModelButton model={null} isSelected={!selectedModel} disabled={true}></ModelButton>
                </Grid>

            </Grid>
        )
    }

    function modelSelectHorizontalScrollLayout() {
        return (
            <Box sx={{overflowX: 'scroll'}}>
                <Stack
                    direction={{xs: 'row', md: 'column'}}
                    justifyContent="space-around"
                    alignItems="center"
                    width={`${(models.length + 1)*90}vw`}
                >
                    {models.map((model, index) => (
                        <Box key={index} width="80vw">
                            <ModelButton model={model} isSelected={model.id === selectedModel}></ModelButton>
                        </Box>
                    ))}

                    <Box width="80vw">
                        <ModelButton model={null} isSelected={!selectedModel} disabled={true}></ModelButton>
                    </Box>

                </Stack>
            </Box>
        )
    }

    return (
        <Box marginBottom={4}>
            <Typography variant="h3">Models</Typography>

            {isDesktop ? 
                modelSelectGridLayout()
            :
                modelSelectHorizontalScrollLayout()
            }
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
