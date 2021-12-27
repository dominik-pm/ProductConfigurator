import { Typography } from '@mui/material'
import React, { useEffect } from 'react'
import { connect } from 'react-redux'
import { useNavigate, useParams } from 'react-router-dom'
import { translate } from '../../lang'
import { fetchConfiguration } from '../../state/configuration/configurationSlice'
import { selectLanguage } from '../../state/language/languageSelectors'
import Configurator from './Configurator/Configurator'

function ConfigurationView({fetchConfiguration, status, error, language }) {

    const navigate = useNavigate()

    // const { configuration, status, error } = useSelector(state => state.configuration)

    const { id } = useParams()
    const productId = Number(id);
    
    // const { products/*, status, error*/ } = useSelector(state => state.product)
    // let product = products.find(p => p.id === productId)
    // if (!product && configuration) {
    //     const {id, name, description, image} = configuration
    //     product = {id, name, description, image}
    // }

    // console.log('loading configuration for product id:', productId)

    if ((!productId && productId !== 0)) {
        console.log('invalid id')
        navigate('/')
    }

    useEffect(() => {
        if (productId || productId === 0) {
            // console.log('calling to fetch configuration')
            fetchConfiguration(productId)
        }
    }, [fetchConfiguration, productId])


    function render() {
        switch (status) {
            case 'loading':
                return renderConfiguration(true)
            case 'succeeded':
                return renderConfiguration()
            case 'failed':
                return renderApiFailed(error)
            default:
                return renderConfiguration(true)
        }
    }

    function renderConfiguration(isLoading = false) {
        return (
            <div>
                <Configurator isLoading={isLoading}></Configurator>
            </div>
        )
    }

    function renderApiFailed(errorMessage) {
        return (
            <div>
                <Typography variant="h2">{translate('failedToLoadConfiguration', language)}</Typography>
                <Typography vairant="body1">{errorMessage}</Typography>
            </div>
        )
    }

    return (
        <div>
            {render()}
        </div>
    )
}

const mapStateToProps = (state) => {
    return {
        status: state.configuration.status,
        error: state.configuration.error,
        language: selectLanguage(state)
    }
}
const mapDispatchToProps = {
    fetchConfiguration
}
export default connect(
    mapStateToProps,
    mapDispatchToProps
)(ConfigurationView)