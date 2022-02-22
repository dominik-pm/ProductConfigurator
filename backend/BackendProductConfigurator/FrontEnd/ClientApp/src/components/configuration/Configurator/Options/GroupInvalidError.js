import { ErrorOutline } from '@mui/icons-material'
import { Tooltip } from '@mui/material'
import React from 'react'

export default function GroupInvalidError({errorMessage}) {
    return (
        <Tooltip 
            title={errorMessage} 
            placement="top"
            arrow
            enterTouchDelay={200}
        >
            <ErrorOutline />
        </Tooltip>
    )
}
