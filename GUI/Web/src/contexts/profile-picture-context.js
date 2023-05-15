import React, { createContext, useEffect, useState } from 'react';

const STORAGE_KEY = 'profilePicture';

const ProfileContext = createContext({
  selectedPicture: null,
  handlePictureUpload: () => {}
});

const ProfileProvider = ({ children }) => {
  const [selectedPicture, setSelectedPicture] = useState(null);

  useEffect(() => {
    const storedPicture = localStorage.getItem(STORAGE_KEY);
    if (storedPicture) {
      setSelectedPicture(storedPicture);
    }
  }, []);

  const handlePictureUpload = (file) => {
    setSelectedPicture(URL.createObjectURL(file));
    localStorage.setItem(STORAGE_KEY, URL.createObjectURL(file));
  };
 

  const contextValue = {
    selectedPicture,
    handlePictureUpload
  };

  return (
    <ProfileContext.Provider value={contextValue}>
      {children}
    </ProfileContext.Provider>
  );
};

export { ProfileContext, ProfileProvider };


