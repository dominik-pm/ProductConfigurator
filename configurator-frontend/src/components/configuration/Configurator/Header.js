import React from 'react'
import { useNavigate } from 'react-router-dom'
import { IconButton } from '@mui/material'
import { ArrowBackIosNew } from '@mui/icons-material'


export default function Header() {
    const navigate = useNavigate()

    return (
        <IconButton onClick={() => navigate('/')}>
            <ArrowBackIosNew></ArrowBackIosNew>
        </IconButton>
    )
}
