import { createContext, useContext, useEffect, useReducer, useRef } from 'react';
import PropTypes from 'prop-types';
import { drawerClasses } from '@mui/material';
import axios from 'axios';
axios.defaults.withCredentials = true;

const HANDLERS = {
  INITIALIZE: 'INITIALIZE',
  SIGN_IN: 'SIGN_IN',
  SIGN_OUT: 'SIGN_OUT'
};

const initialState = {
  isAuthenticated: false,
  isLoading: true,
  user: null
};

const handlers = {
  [HANDLERS.INITIALIZE]: (state, action) => {
    const user = action.payload;

    return {
      ...state,
      ...(
        // if payload (user) is provided, then is authenticated
        user
          ? ({
            isAuthenticated: true,
            isLoading: false,
            user
          })
          : ({
            isLoading: false
          })
      )
    };
  },
  [HANDLERS.SIGN_IN]: (state, action) => {
    const user = action.payload;

    return {
      ...state,
      isAuthenticated: true,
      user
    };
  },
  [HANDLERS.SIGN_OUT]: (state) => {
    return {
      ...state,
      isAuthenticated: false,
      user: null,
    };
  }
};

const reducer = (state, action) => (
  handlers[action.type] ? handlers[action.type](state, action) : state
);

// The role of this context is to propagate authentication state through the App tree.

export const AuthContext = createContext({ undefined });

export const AuthProvider = (props) => {
  const { children } = props;
  const [state, dispatch] = useReducer(reducer, initialState);
  const initialized = useRef(false);

  const initialize = async () => {
    // Prevent from calling twice in development mode with React.StrictMode enabled
    if (initialized.current) {
      return;
    }

    initialized.current = true;

    let isAuthenticated = false;

    try {
      isAuthenticated = window.sessionStorage.getItem('authenticated') === 'true';
    } catch (err) {
      console.error(err);
    }

    if (isAuthenticated) {
      const user = {
        id: '5e86809283e28b96d2d38537',
        avatar: '/assets/avatars/avatar-anika-visser.png',
        name: 'Anika Visser',
        email: 'anika.visser@devias.io'
      };

      dispatch({
        type: HANDLERS.INITIALIZE,
        payload: user
      });
    } else {
      dispatch({
        type: HANDLERS.INITIALIZE
      });
    }
  };

  useEffect(
    () => {
      initialize();
    },
    // eslint-disable-next-line react-hooks/exhaustive-deps
    []
  );

  const skip = () => {
    try {
      window.sessionStorage.setItem('authenticated', 'true');
    } catch (err) {
      console.error(err);
    }

    const user = {
      id: '5e86809283e28b96d2d38537',
      avatar: '/assets/avatars/avatar-anika-visser.png',
      name: 'Anika Visser',
      email: 'anika.visser@devias.io'
    };

    dispatch({
      type: HANDLERS.SIGN_IN,
      payload: user
    });
  };

  const signIn = async (email, password) => {
    const queryParams = new URLSearchParams({
      password: password,
      email: email,
    });
    const response = await fetch(`https://localhost:7094/LogInToStratusService?${queryParams}`, {credentials:'include'});
    const data = await response.json();
    console.log(data.message);
    //
    //if (email !== 'demo@devias.io' || password !== 'Password123!') {
    //  throw new Error('Please check your email and password');
    //}
    if (response.ok === false){
      throw new Error('Please check your email and password');
    } 
    try {
      window.sessionStorage.setItem('authenticated', 'true');
      Cookies.set('userDBEmail', email);
    
    } catch (err) {
      console.error(err);
    }

    const user = {
      id: response.id,
      //avatar: '/assets/avatars/avatar-anika-visser.png',
      name: response.name,
      email: email
    };

    dispatch({
      type: HANDLERS.SIGN_IN,
      payload: user
    });
  };

  const signUp = async (email, name, password, accessKey, secretKey, selectedRegion) => {
    const queryParams = new URLSearchParams({
      username: name,
      password: password,
      email: email,
      accessKey: accessKey,
      secretKey: secretKey,
      region: selectedRegion
    });

    // Send the HTTP GET request
    console.log(document.cookie);
    const response = await fetch(`https://localhost:7094/RegisterToStratusService?${queryParams}`, {credentials:'include'});
    const data = await response.json();
    if (response.ok === false){
      console.log(data.message);

      throw new Error(data.message);
    }
         
    try {

      window.sessionStorage.setItem('authenticated', 'true');
      //Cookies.set('userDBEmail', email);

      const response = axios.head(`https://localhost:7094/RegisterToAlerts`);
      const data = await response.json();
      if (response.ok === false){
        console.log(data.message);
  
        throw new Error(data.message);
      }
    } 
    catch (err) 
    {
      console.error(err);
    }

    const user = {
      id: response.id,
      avatar: '/assets/avatars/avatar-anika-visser.png',
      name: response.name,
      email: email
    };

    dispatch({
      type: HANDLERS.SIGN_IN,
      payload: user
    });
  };

  const signOut = async () => {
    try {
      // Call the logout endpoint on the server
      await axios.get('https://localhost:7094/LogOutFromStratusService', { credentials: 'include' });
  
      // Delete the "Stratus" cookie from the web browser
      document.cookie = 'Stratus=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;';
      window.sessionStorage.setItem('authenticated', 'false');
      // Dispatch the SIGN_OUT action to update the Redux store
      dispatch({
        type: HANDLERS.SIGN_OUT
      });
    } catch (err) {
      console.error(err);
    }
  };

  return (
    <AuthContext.Provider
      value={{
        ...state,
        skip,
        signIn,
        signUp,
        signOut
      }}
    >
      {children}
    </AuthContext.Provider>
  );
};

AuthProvider.propTypes = {
  children: PropTypes.node
};

export const AuthConsumer = AuthContext.Consumer;

export const useAuthContext = () => useContext(AuthContext);
