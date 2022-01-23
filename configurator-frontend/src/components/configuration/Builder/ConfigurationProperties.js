import { Grid, InputAdornment, TextField, Typography } from '@mui/material'
import { Box } from '@mui/system'
import React, { useState } from 'react'
import { connect } from 'react-redux'
import { translate } from '../../../lang'
import { setBasePrice, setDescription } from '../../../state/configurationBuilder/builderSlice'
import { selectLanguage } from '../../../state/language/languageSelectors'

function ConfigurationProperties({ setDescription, setPrice, language }) {

    const [priceError, setPriceError] = useState(false)

    function handleDescriptionChanged(event) {
        const desc = event.target.value
        if (desc) {
            setDescription(desc)
        }
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
        
        <Grid item>
            <Typography variant="h3">Product Properties</Typography>
            <Grid container columnGap={0} rowGap={2}>

                <Grid item xs={12} md={2}>
                    <TextField 
                        fullWidth
                        label={translate('basePrice', language)}
                        variant="outlined"
                        onChange={handleBasePriceChanged}
                        error={priceError}
                        defaultValue={0}
                        type="number"
                        InputProps={{
                            inputProps: {
                                min: 0
                            },
                            startAdornment: <InputAdornment position='start'>€</InputAdornment>
                        }}
                    />
                </Grid>

                <Grid item xs={12} md={10}>
                    <TextField 
                        fullWidth
                        label={translate('description', language)}
                        variant="outlined"
                        onChange={handleDescriptionChanged}
                        multiline
                        maxRows={4}
                    />
                </Grid>

            </Grid>
        </Grid>
    )
}

const mapStateToProps = (state) => ({
    language: selectLanguage(state)
})
const mapDispatchToProps = {
    setDescription: setDescription,
    setPrice: setBasePrice
}
export default connect(
    mapStateToProps, 
    mapDispatchToProps
)(ConfigurationProperties)
