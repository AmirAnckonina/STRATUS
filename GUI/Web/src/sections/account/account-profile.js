import {
  Avatar,
  Box,
  Button,
  Card,
  CardActions,
  CardContent,
  Divider,
  Typography
} from '@mui/material';
import { ProfileProvider, ProfileContext } from 'src/contexts/profile-picture-context';
import React, { useContext, useEffect, useState } from 'react';
import axios from 'axios';


export const AccountProfile = () => {
  const { selectedPicture, handlePictureUpload } = useContext(ProfileContext);
  const [user, setUser] = useState({});

  function stringToColor(string) {
    let hash = 0;
    let i;
  
    /* eslint-disable no-bitwise */
    for (i = 0; i < string.length; i += 1) {
      hash = string.charCodeAt(i) + ((hash << 5) - hash);
    }
  
    let color = '#';
  
    for (i = 0; i < 3; i += 1) {
      const value = (hash >> (i * 8)) & 0xff;
      color += `00${value.toString(16)}`.slice(-2);
    }
    /* eslint-enable no-bitwise */
  
    return color;
  }
  
  function stringAvatar(name) {
    return {
      sx: {
        bgcolor: name ? stringToColor(name) : null,
      },
      children: `${name ? name[0] : ''}`,
    };
  }

  useEffect(() => {
  
    axios
      .get('https://localhost:7094/GetUserByEmail')
      .then((response) => {
        console.log('data:', response.data);
        setUser(response.data.data);
      })
      .catch((error) => {
        console.error('Error fetching data:', error);
      });
  }, []); 

return (
  <Card>
    <CardContent>
      <Box
        sx={{
          alignItems: 'center',
          display: 'flex',
          flexDirection: 'column'
        }}
      >
        <Avatar
          {...stringAvatar(user.username)}
        />
        <Typography
          gutterBottom
          variant="h6"
        >
          {user.username} {user.lastName}
        </Typography>
        <Typography
          color="text.secondary"
          variant="body2"
        >
          {user.city} {user.country}
        </Typography>
        <Typography
          color="text.secondary"
          variant="body2"
        >
          {user.email}
        </Typography>
      </Box>
    </CardContent>
    <Divider />
    <CardActions>
    <Button fullWidth variant="text" onClick={() => document.getElementById('upload-picture-input').click()}>
    Upload picture
</Button>
<input
  id="upload-picture-input"
  type="file"
  style={{ display: 'none' }}
  accept="image/*"
  onChange={(event) => handlePictureUpload(event.target.files[0])}
/>
    </CardActions>
  </Card>
)};
