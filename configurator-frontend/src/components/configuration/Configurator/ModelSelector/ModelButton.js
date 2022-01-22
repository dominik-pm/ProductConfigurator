import { Box, Button, Typography } from '@mui/material'
import React from 'react'
import { connect } from 'react-redux'
import { setModel } from '../../../../state/configuration/configurationSlice'

function ModelButton({ model, isSelected = false, disabled = false, selectModel }) {

    const { modelName, description, options } = model

    function handleClick() {
        if (!disabled) {
            selectModel(modelName)
        }
    }

    return (
        <Button variant={isSelected ? "contained" : "outlined"} onClick={handleClick} disabled={false}>
            <Box>
                <Typography variant="body1">
                    {modelName}
                </Typography>
                <Typography variant="body2">
                    {description}
                </Typography>
            </Box>
        </Button>
    )
}

const mapStateToProps = (state) => ({

})
const mapDispatchToProps = {
    selectModel: setModel
}
export default connect(
    mapStateToProps, 
    mapDispatchToProps
)(ModelButton)
