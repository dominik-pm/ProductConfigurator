import { createSelector } from '@reduxjs/toolkit'

const selectBuilderLanguages = (state) =>                       state.builder.configuration.languages
const selectCurrentBuilderLanguage = (state) =>                 state.builder.currentLanguage

export const selectBuilderConfiguration = (state) =>            state.builder.configuration
export const selectBuilderStatus = (state) =>                   state.builder.status
export const selectBuilderError = (state) =>                    state.builder.error
export const selectBuilderInputLanguage = (state) =>            state.builder.currentLanguage
export const selectBuilderAvailableImages = (state) =>          state.builder.availableImages

export const selectBuilderImages = (state) =>                   state.builder.configuration.images
export const selectBuilderSections = (state) =>                 state.builder.configuration.optionSections
export const selectBuilderGroups = (state) =>                   state.builder.configuration.optionGroups
export const selectBuilderOptions = (state) =>                  state.builder.configuration.options
export const selectBuilderModels = (state) =>                   state.builder.configuration.rules.models
export const selectBuilderDefaultModel = (state) =>             state.builder.configuration.rules.defaultModel
export const selectBuilderBasePrice = (state) =>                state.builder.configuration.rules.basePrice
export const selectBuilderPriceList = (state) =>                state.builder.configuration.rules.priceList
export const selectBuilderOptionRequirements = (state) =>       state.builder.configuration.rules.requirements
export const selectBuilderOptionIncompatibilities = (state) =>  state.builder.configuration.rules.incompatibilities
export const selectBuilderGroupRequirements = (state) =>        state.builder.configuration.rules.groupRequirements || []

const selectBuilderName = (state) =>                                    state.builder.configuration.languages[state.builder.currentLanguage].name || ''
const selectBuilderDescription = (state) =>                             state.builder.configuration.languages[state.builder.currentLanguage].description || ''
export const selectBuilderOptionsFromCurrentLanguage = (state) =>       state.builder.configuration.languages[state.builder.currentLanguage].options || []
export const selectBuilderGroupsFromCurrentLanguage = (state) =>        state.builder.configuration.languages[state.builder.currentLanguage].optionGroups || []
export const selectBuilderSectionsFromCurrentLanguage = (state) =>      state.builder.configuration.languages[state.builder.currentLanguage].optionSections || []

const selectName = (state, name) =>                             name

// also updates cached value when builder language changes
export const getBuilderName = createSelector([selectBuilderName, selectCurrentBuilderLanguage], (builderName, lang) => {
    return builderName ? builderName : ''
})
// also updates cached value when builder language changes
export const getBuilderDescription = createSelector([selectBuilderDescription, selectCurrentBuilderLanguage], (builderDescription, lang) => {
    return builderDescription ? builderDescription : ''
})

export const getBuilderOptionById = createSelector([selectName, selectBuilderOptionsFromCurrentLanguage], (optionId, options) => {
    const option = options.find(o => o.id === optionId)
    return option ? option : null
})
export const getBuilderOptionProductNumber = createSelector([selectName, selectBuilderOptions], (optionId, options) => {
    const option = options.find(o => o.id === optionId)
    return option ? option.productNumber : ''
})
export const getBuilderOptionPrice = createSelector([selectName, selectBuilderPriceList], (optionId, priceList) => {
    const price = priceList[optionId]
    return price ? price : 0
})
export const getBuilderOptionRequirementsByOptionId = createSelector([selectName, selectBuilderOptionRequirements], (optionId, requirements) => {
    const optionReq = requirements[optionId]
    return optionReq ? optionReq : []
})
export const getBuilderOptionIncompatibilitiesByOptionId = createSelector([selectName, selectBuilderOptionIncompatibilities], (optionId, incompatibilities) => {
    const optionIncomp = incompatibilities[optionId]
    return optionIncomp ? optionIncomp : []
})

export const getBuilderSectionById = createSelector([selectName, selectBuilderSections], (sectionId, sections) => {
    const section = sections.find(s => s.id === sectionId)
    return section ? section : null
})
export const getBuilderGroupsInSection = createSelector([selectName, selectBuilderSections], (sectionId, sections) => {
    const section = sections.find(s => s.id === sectionId)
    return section ? section.optionGroupIds : []
})

export const getBuilderGroupById = createSelector([selectName, selectBuilderGroups], (groupId, groups) => {
    const group = groups.find(g => g.id === groupId)
    return group ? group : null
})
export const getBuilderGroupNameByOptionId = createSelector([selectName, selectBuilderGroups, selectBuilderGroupsFromCurrentLanguage], (optionId, groups, groupLangObj) => {
    const group = groups.find(g => g.optionIds.includes(optionId))
    return group ? groupLangObj.find(g => g.id === group.id).name : ''
})
export const getBuilderGroupRequirementsByGroupId = createSelector([selectName, selectBuilderGroupRequirements], (groupId, requirements) => {
    const groupReq = requirements[groupId]
    return groupReq ? groupReq : []
})

const getBuilderLanguageObject = createSelector([selectBuilderLanguages, selectCurrentBuilderLanguage], (languages, language) => {
    return languages[language]
})


export const getDoesSectionExist = createSelector([selectBuilderSections, selectName], (sections, sectionName) => {
    return sections.find(s => s.id.toUpperCase() === sectionName.toUpperCase()) ? true : false
})
export const getDoesGroupdExist = createSelector([selectBuilderGroups, selectName], (groups, groupName) => {
    return groups.find(g => g.id.toUpperCase() === groupName.toUpperCase()) ? true : false
})
export const getDoesOptionExist = createSelector([selectBuilderOptions, selectName], (options, optionName) => {
    return options.find(o => o.id.toUpperCase() === optionName.toUpperCase()) ? true : false
})

const getGroupPropertiesFromBuilderGroup = createSelector([getBuilderLanguageObject, selectName], (langObj, groupId) => {
    const group = langObj.optionGroups.find(g => g.id === groupId)
    return group || null
})
export const getGroupNameFromBuilderGroup = createSelector([getGroupPropertiesFromBuilderGroup], (group) => group ? group.name : '')
export const getGroupDescriptionFromBuilderGroup = createSelector([getGroupPropertiesFromBuilderGroup], (group) => group ? group.description : '')

const getModelPropertiesFromBuilderModel = createSelector([getBuilderLanguageObject, selectName], (langObj, modelObj) => {
    const model = langObj.models.find(m => m.id === modelObj.id)
    return model || null
})
export const getModelNameFromBuilderModel = createSelector([getModelPropertiesFromBuilderModel], (model) => model ? model.name : 'c')
export const getModelDescriptionFromBuilderModel = createSelector([getModelPropertiesFromBuilderModel], (model) => model ? model.description : 'c')


export const extractProductNumberFromBuilderOption = (option) =>    option.productNumber || ''
export const extractGroupIdFromBuilderOption = (option) =>          option.groupId || ''

export const extractModelIdFromBuilderModel = (model) =>            model.id || ''
export const extractModelNameFromBuilderModel = (model) =>          model.name || ''
export const extractModelOptionsFromBuilderModel = (model) =>       model.optionIds || []

export const extractOptionsFromBuilderGroup = (group) =>            group.optionIds || []
export const extractGroupNameFromBuilderGroupId = (group) =>        group.name || ''

export const extractGroupsFromBuilderSection = (section) =>         section.optionGroupIds || []