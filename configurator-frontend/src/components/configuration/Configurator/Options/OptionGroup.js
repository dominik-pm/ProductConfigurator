import { Typography } from '@mui/material'
import { Box } from '@mui/system'
import React from 'react'
import Option from './Option'
import './OptionGroup.css'

export default function OptionGroup({group}) {
    const {name, description, optionIds} = group

    return (
        <Box className="OptionGroup">
            <Typography variant="h2">{name}</Typography>
            <Typography variant="subtitle2">{description}</Typography>
            <Box className="OptionContainer">
                {
                    optionIds.map(optionId => (
                        <Option key={optionId} optionId={optionId}></Option>
                    ))
                }
            </Box>
        </Box>
    )
}
