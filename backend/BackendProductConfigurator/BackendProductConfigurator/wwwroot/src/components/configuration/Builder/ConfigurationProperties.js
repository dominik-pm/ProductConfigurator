import { Grid, InputAdornment, TextField, Typography } from '@mui/material'
import React, { useState, useEffect } from 'react'
import { connect } from 'react-redux'
import { translate } from '../../../lang'
import { getBuilderDescription, getBuilderName, selectBuilderBasePrice } from '../../../state/configurationBuilder/builderSelectors'
import { setBasePrice, setDescription, setName } from '../../../state/configurationBuilder/builderSlice'
import { selectLanguage } from '../../../state/language/languageSelectors'

function ConfigurationProperties({ name, description, basePrice, setName, setDescription, setPrice, language }) {

    const [priceError, setPriceError] = useState(false)
    const [nameInput, setNameInput] = useState(name)
    const [descriptionInput, setDescriptionInput] = useState(description)
    
    useEffect(() => {
        // when the description updates, also update the value for the input field
        // -> language could change so the value for the other languages name might be different
        setNameInput(name)
    }, [name, setNameInput])
    
    useEffect(() => {
        setDescriptionInput(description)
    }, [description, setNameInput])

    function handleNameChanged(event) {
        if (nameInput) setName(nameInput)
    }

    function handleDescriptionChanged(event) {
        if (descriptionInput) setDescription(descriptionInput)
    }

    function handleBasePriceChanged(event) {
        const price = Number(event.target.value)

        if (price || price === 0) {
            setPriceError(false)
            setPrice(price)
        } else {
            setPriceError(true)
        }
    }

    return (
        <Grid item container rowGap={2}>
            <Grid item>
                <Typography variant="h3">{translate('productProperties', language)}</Typography>
            </Grid>

            <Grid item container rowSpacing={2} columnSpacing={2}>
                <Grid item xs={12} sm={8} md={3}>
                    <TextField 
                        fullWidth
                        label={translate('configurationName', language)}
                        variant="outlined"
                        value={nameInput}
                        onChange={(e) => setNameInput(e.target.value)}
                        onBlur={handleNameChanged}
                    />
                </Grid>

                <Grid item xs={12} flexGrow={1} sm={4} md={2}>
                    <TextField
                        fullWidth
                        label={translate('basePrice', language)}
                        variant="outlined"
                        onChange={handleBasePriceChanged}
                        error={priceError}
                        defaultValue={basePrice}
                        type="number"
                        InputProps={{
                            inputProps: {
                                min: 0
                            },
                            startAdornment: <InputAdornment position='start'>€</InputAdornment>
                        }}
                    />
                </Grid>

                <Grid item flexGrow={1}>
                    <TextField 
                        fullWidth
                        label={translate('description', language)}
                        variant="outlined"
                        value={descriptionInput}
                        onChange={(e) => setDescriptionInput(e.target.value)}
                        onBlur={handleDescriptionChanged} // opnly change description when done (if called on every change -> there would be too many state calls that would result in lag)
                        multiline
                        maxRows={4}
                    />
                </Grid>
            </Grid>
        </Grid>
    )
}

const mapStateToProps = (state) => ({
    name: getBuilderName(state),
    description: getBuilderDescription(state),
    basePrice: selectBuilderBasePrice(state),
    language: selectLanguage(state)
})
const mapDispatchToProps = {
    setName: setName,
    setDescription: setDescription,
    setPrice: setBasePrice
}
export default connect(
    mapStateToProps, 
    mapDispatchToProps
)(ConfigurationProperties)
