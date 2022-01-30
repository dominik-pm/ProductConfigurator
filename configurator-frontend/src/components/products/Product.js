import React from 'react'
import { ImageListItem, ImageListItemBar } from '@mui/material'
import { useNavigate } from 'react-router'

export default function Product({product}) {
    const navigate = useNavigate()

    const { configId, name, description, images } = product

    const image = images[0]

    let imageSource = ''

    try {
        const src = require(`../../assets/img/${image.replace('./', '')}`)
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

        <ImageListItem key={imageSource} sx={{width: '100%', ':hover': {cursor: 'pointer'}}} onClick={() => handleClick(configId)}>
            <img
                width="100%"
                src={`${imageSource}?w=248&fit=crop&auto=format`}
                srcSet={`${imageSource}?w=248&fit=crop&auto=format&dpr=2 2x`}
                alt={name}
                loading="lazy"
            />
            <ImageListItemBar
                title={name}
                subtitle={description}
            />
        </ImageListItem>
    )
}
