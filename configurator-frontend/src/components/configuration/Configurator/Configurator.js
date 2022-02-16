import { Done, RestartAlt, SaveAs } from '@mui/icons-material'
import { Box, Grid, IconButton, ImageList, ImageListItem, Tooltip, Typography } from '@mui/material'
import React from 'react'
import { connect } from 'react-redux'
import { useNavigate } from 'react-router-dom'
import { postOrderConfiguredProduct } from '../../../api/productsAPI'
import { requestSaveConfiguration } from '../../../api/userAPI'
import { getImageSource } from '../../../App'
import { translate } from '../../../lang'
import { alertTypes, openAlert } from '../../../state/alert/alertSlice'
import { getCurrentPrice, selectConfigurationDescription, selectConfigurationId, selectConfigurationImages, selectConfigurationName, selectSelectedModel, selectSelectedOptions } from '../../../state/configuration/configurationSelectors'
import { resetActiveConfiguration } from '../../../state/configuration/configurationSlice'
import { confirmDialogOpen } from '../../../state/confirmationDialog/confirmationSlice'
import { inputDialogOpen } from '../../../state/inputDialog/inputDialogSlice'
import { selectLanguage } from '../../../state/language/languageSelectors'
import { selectIsAuthenticated } from '../../../state/user/userSelector'
import { openLogInDialog } from '../../header/LoginButton'
import { Slide } from 'react-slideshow-image'
import Loader from '../../Loader'
import ModelSelector from './ModelSelector/ModelSelector'

import OptionTabs from './OptionTabs'
import Summary from './SidePanel/Summary'
import 'react-slideshow-image/dist/styles.css'
// import Summary from './SidePanel/Summary'


/*

optionGroups: [
    {
        id: 'COLOR_GROUP',
        name: 'Color',
        description: 'the exterior color of the car',
        optionIds: [
            'BLUE', 'YELLOW', 'GREEN'
        ]
    }
]

*/

function Configurator({ isLoggedIn, configurationName, configurationDescription, configurationImages, configurationId, selectedOptions, price, model, isLoading, resetConfig, openConfirm, openInputDialog, openLogInDialog, openAlert, language }) {

    const navigate = useNavigate()

    function handleSaveClicked() {
        if (!isLoggedIn) {
            openLogInDialog()
            return
        }

        const data = {
            configurationName: {name: translate('configurationName', language), value: '' }
        }
        const title = translate('saveConfiguration', language)

        openInputDialog(title, data, (data) => {
            const configurationName = data.configurationName.value

            requestSaveConfiguration(configurationId, configurationName, selectedOptions)
            .then(res => {
                openAlert(`${translate('savedConfiguration', language)}: ${configurationName}!`, alertTypes.SUCCESS)
                console.log(res)
            })
            .catch(err => {
                openAlert(`Error: ${err}`, alertTypes.ERROR)
                console.log(err)
            })
        })
    }

    function handleResetClicked() {
        openConfirm(translate('resetConfigurationPrompt', language), {}, null, () => {
            resetConfig()
        })
    }

    function handleFinishClicked() {
        if (!isLoggedIn) {
            openLogInDialog()
            return
        }

        // confirm dialog that shows the summary
        openConfirm('', {}, <Summary configurationId={configurationId} selectedOptions={selectedOptions}></Summary>, () => {
            // when confirmed -> prompt with configuration name and then send the request to order the configuration
            promptOrderConfiguration()
        })
    }
    function promptOrderConfiguration() {
        const data = {
            configurationName: {name: translate('configurationName', language), value: '' }
        }
        const title = translate('finishConfiguration', language)
        openInputDialog(title, data, (data) => {
            const configurationName = data.configurationName.value

            postOrderConfiguredProduct(configurationId, configurationName, selectedOptions, price, model)
            .then(res => {
                openAlert(`${translate('successOrderedConfiguration', language)}!`, alertTypes.SUCCESS)
                console.log(res)
                navigate('/account/ordered')
            })
            .catch(err => {
                openAlert(`Error: ${err}`, alertTypes.ERROR)
                console.log(err)
            })
        })
    }


    function renderConfigurator() {

        if (isLoading) {
            return (
                <Loader></Loader>
            )
        }

        return (
            <div>
                {/* Configurator header */}
                <Grid container justifyContent="flex-end">
                    <Box sx={{flexGrow: 1}}>
                        <Typography variant="h2">{translate('configureYour', language)} {configurationName}</Typography>
                        <Typography variant="subtitle1">{configurationDescription}</Typography>
                    </Box>

                    <Grid item sx={{paddingTop: 2, justifySelf: 'flex-end'}}>
                        <Tooltip title={translate('saveConfiguration', language)}>
                            <IconButton 
                                variant="contained" 
                                onClick={handleSaveClicked}
                                >
                                <SaveAs />
                            </IconButton>
                        </Tooltip>

                        <Tooltip title={translate('resetConfiguration', language)}>
                            <IconButton
                                variant="contained" 
                                onClick={handleResetClicked}
                            >
                                <RestartAlt />
                            </IconButton>
                        </Tooltip>

                        <Tooltip title={translate('finishConfiguration', language)}>
                            <IconButton 
                                variant="contained" 
                                onClick={handleFinishClicked}
                                >
                                <Done />
                            </IconButton>
                        </Tooltip>
                    </Grid>
                </Grid>

                {/* Images */}
                {/* <Box mb={4}>
                    <Slide easing="ease">
                        {configurationImages.map((image, index) => (
                            <div key={index} className="each-slide">
                                <div style={{
                                    height: '60vw',
                                    maxHeight: '600px', 
                                    backgroundImage: `url(${getImageSource(image)})`,
                                    backgroundSize: 'cover',
                                    backgroundPosition: 'center'
                                }}>
                                </div>
                            </div>
                        ))}
                    </Slide>
                </Box> */}

                {/* <ImageList sx={{ width: '100%', height: '350px' }} cols={{sm: 1, lg: 4}} rowHeight={350}>
                    {[...configurationImages, ...configurationImages, ...configurationImages].map((item) => (
                        <ImageListItem key={item}>
                            <img
                                {...srcset(getImageSource(item), '100%', 350, 1, 1)}
                                //src={`${getImageSource(item)}`}
                                //srcSet={`${getImageSource(item)}`}
                                alt={configurationName}
                                loading="lazy"
                            />
                        </ImageListItem>
                    ))}
                </ImageList> */}

                {/* Models */}
                <ModelSelector></ModelSelector>

                <Grid container>
                    <OptionTabs></OptionTabs>
                </Grid>
            </div>

        )
    }

    function srcset(image, width, height, rows = 1, cols = 1) {
        return {
            src: `${image}?w=${width * cols}&h=${height * rows}&fit=crop&auto=format`,
            srcSet: `${image}?w=${width * cols}&h=${height * rows}&fit=crop&auto=format&dpr=2 2x`
        }
    }

    return (
        <div>
            {renderConfigurator()}
        </div>
    )
}

const mapStateToProps = (state) => ({
    configurationName: selectConfigurationName(state),
    configurationDescription: selectConfigurationDescription(state),
    configurationImages: selectConfigurationImages(state),
    configurationId: selectConfigurationId(state),
    selectedOptions: selectSelectedOptions(state),
    price: getCurrentPrice(state),
    model: selectSelectedModel(state),
    language: selectLanguage(state),
    isLoggedIn: selectIsAuthenticated(state)
})
const mapDispatchToProps = {
    resetConfig: resetActiveConfiguration,
    openConfirm: confirmDialogOpen,
    openInputDialog: inputDialogOpen,
    openLogInDialog: openLogInDialog,
    openAlert: openAlert
}
export default connect(
    mapStateToProps,
    mapDispatchToProps
)(Configurator)
