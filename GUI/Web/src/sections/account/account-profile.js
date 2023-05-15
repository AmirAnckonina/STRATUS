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
import React, { useContext } from 'react';

const user = {
  avatar: '/assets/avatars/avatar-anika-visser.png',
  city: 'Los Angeles',
  country: 'USA',
  jobTitle: 'Senior Developer',
  name: 'Anika Visser',
  timezone: 'GTM-7'
};


export const AccountProfile = () => {
  const { selectedPicture, handlePictureUpload } = useContext(ProfileContext);


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
          src={selectedPicture ? selectedPicture : user.avatar}
          sx={{
            height: 80,
            mb: 2,
            width: 80
          }}
        />
        <Typography
          gutterBottom
          variant="h5"
        >
          {user.name}
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
          {user.timezone}
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
