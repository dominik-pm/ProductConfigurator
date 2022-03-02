import React, { useEffect } from 'react'
import { Box, Checkbox, FormControl, Grid, InputLabel, ListItemText, MenuItem, OutlinedInput, Select, Typography } from '@mui/material'
import { translate } from '../../../lang'
import { selectLanguage } from '../../../state/language/languageSelectors'
import { connect } from 'react-redux'
import { loadImages, setImages } from '../../../state/configurationBuilder/builderSlice'
import { selectBuilderAvailableImages, selectBuilderImages } from '../../../state/configurationBuilder/builderSelectors'

function ImageSelect({ selectedImages, allImages, loadImages, setImages, language }) {

    useEffect(() => {
        loadImages()
    }, [loadImages])
    

    function handleChangeImageSelection(event) {
        const {
            target: { value },
        } = event

        // On autofill we get a stringified value. 
        const newImageSelection = typeof value === 'string' ? value.split(',') : value

        setImages(newImageSelection)
    }
    
    return (
        <Grid item>
            <Box>
                <Typography variant="h3">{translate('productImages', language)}</Typography>
            </Box>
            <Box>
                <FormControl sx={{ m: 1, width: 300 }}>
                    <InputLabel id={'images-label'}>{translate('images', language)}</InputLabel>
                    <Select
                        labelId="images-label"
                        multiple
                        value={selectedImages}
                        onChange={handleChangeImageSelection}
                        input={<OutlinedInput label={translate('images', language)} />}
                        renderValue={(selectedImages) => {
                                return selectedImages.join(', ').replace('*', '/')
                            }
                        }
                    >
                        {allImages.map((image) => (
                            <MenuItem key={image} value={image}>
                                <Checkbox checked={selectedImages.indexOf(image) > -1} />
                                <ListItemText primary={`${image.replace('*', '/')}`} />
                            </MenuItem>
                        ))}
                    </Select>
                </FormControl>
            </Box>
        </Grid>
    )
}

const mapStateToProps = (state) => ({
    selectedImages: selectBuilderImages(state),
    allImages: selectBuilderAvailableImages(state),
    language: selectLanguage(state)
})
const mapDispatchToProps = {
    loadImages,
    setImages
}
export default connect(
    mapStateToProps, 
    mapDispatchToProps
)(ImageSelect)
