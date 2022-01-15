import { Box, Typography } from '@mui/material'
import React from 'react'

export default function BuilderOption({ optionId }) {
    return (
        <Box>
            <Typography variant="body1">{optionId}</Typography>
        </Box>
    )
}
