import React from 'react'
import { ImageListItem, ImageListItemBar } from '@mui/material'
import { useNavigate } from 'react-router'
import './Product.css'

export default function Product({product}) {
    const navigate = useNavigate()

    const {id, name, description, image} = product

    let imageSource = ''

    try {
        const src = require(`../../assets/img/${image}`)
        imageSource = src.default
    } catch (err) {
        console.log(`image '${image}' no found!`)
        const src = require(`../../assets/img/notfound.jpg`)
        imageSource = src.default
    }
    
    function handleClick(id) {
        console.log(`clicked on: ${name} (id ${id})`)
        navigate(`/configuration/${id}`)
    }

    return (
        // <ButtonBase className="Product" onClick={() => handleClick(id)}>
        //     <Box>
        //         <Typography variant="h2" color="main">{name}</Typography>
        //         <Typography>{description}</Typography>
        //         <img src={imageSource} alt={`${name}`} />
        //     </Box>
        // </ButtonBase>

        <ImageListItem key={imageSource} className="Product" onClick={() => handleClick(id)}>
        <img
            src={`${imageSource}?w=248&fit=crop&auto=format`}
            srcSet={`${imageSource}?w=248&fit=crop&auto=format&dpr=2 2x`}
            alt={name}
            loading="lazy"
        />
        <ImageListItemBar
            title={name}
            subtitle={description}
            // actionIcon={
            // <IconButton
            //     sx={{ color: 'rgba(255, 255, 255, 0.54)' }}
            //     aria-label={`info about ${item.title}`}
            // >
            //     <InfoIcon />
            // </IconButton>
            // }
        />
        </ImageListItem>
    )
}
