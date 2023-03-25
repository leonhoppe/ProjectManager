export interface Language {
  // Navigation
  selectLang: string,
  design: string,
  profileSettings: string,
  dashboard: string,
  logout: string,
  addProject: string,
  projects: string,
  running: string,
  stopped: string,

  // Dashboard
  welcome: string,
  noProjects: string,
  open: string,
  edit: string,
  delete: string,
  deleteProjQuestion: string,

  // Popups
  name: string,
  domain: string,
  cancel: string,
  createProject: string,
  editProject: string,
  updateProject: string,
  deleteProject: string,

  // Profile
  profile: string,
  profileSub: string,
  email: string,
  username: string,
  password: string,
  passwordRepeat: string,
  updateAccount: string,
  deleteAccount: string,
  saveChanges: string,
  updateFailed: string,
  accountUpdated: string,
  deleteQuestion: string,
  deleteWarning: string,
  accountDeleted: string,
  submit: string,

  // Login / Register
  login: string,
  register: string,
  noAccount: string,
  alreadyAccount: string,
  valueToLong: string,
  validEmail: string,
  isRequired: string,
  emailOrPasswordWrong: string,
  passwordsDontMatch: string,
  registerFailed: string,
}
