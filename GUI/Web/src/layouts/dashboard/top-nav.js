import PropTypes from 'prop-types';
import React, { useContext, useState, useEffect } from 'react';
import BellIcon from '@heroicons/react/24/solid/BellIcon';
import UsersIcon from '@heroicons/react/24/solid/UsersIcon';
import Bars3Icon from '@heroicons/react/24/solid/Bars3Icon';
import MagnifyingGlassIcon from '@heroicons/react/24/solid/MagnifyingGlassIcon';
import {
  Avatar,
  Badge,
  Box,
  IconButton,
  Stack,
  SvgIcon,
  Tooltip,
  useMediaQuery,
  Popover,
  List,
  ListItem,
  Alert,
} from '@mui/material';
import { alpha } from '@mui/material/styles';
import { usePopover } from 'src/hooks/use-popover';
import { AccountPopover } from './account-popover';
import { ProfileContext } from 'src/contexts/profile-picture-context';
import axios from 'axios';

const SIDE_NAV_WIDTH = 280;
const TOP_NAV_HEIGHT = 64;

export const TopNav = (props) => {
  const { onNavOpen } = props;
  const [user, setUser] = useState({});
  const lgUp = useMediaQuery((theme) => theme.breakpoints.up('lg'));
  const accountPopover = usePopover();
  const [notifications, setNotifications] = useState([
    { id: 1, message: "CPU: Detected 21.3% usage at 27/08/23 18:32" },
    { id: 2, message: "Memory: Detected 32.3% usage at 27/08/23 19:32" },
    // Add more dummy notifications as needed
  ]);
  const [notificationPopoverOpen, setNotificationPopoverOpen] = useState(false);

  const handleNotificationPopoverOpen = () => {
    setNotificationPopoverOpen(true);
  };
  
  const handleNotificationPopoverClose = () => {
    setNotificationPopoverOpen(false);
  };
  
  
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
        console.log('dataaa:', response.data);
        setUser(response.data.data);
       })
      .catch((error) => {
        console.error('Error fetching data:', error);
      });
  }, []);

  return (
    <>
      <Box
        component="header"
        sx={{
          backdropFilter: 'blur(6px)',
          backgroundColor: (theme) => alpha(theme.palette.background.default, 0.8),
          position: 'sticky',
          left: {
            lg: `${SIDE_NAV_WIDTH}px`
          },
          top: 0,
          width: {
            lg: `calc(100% - ${SIDE_NAV_WIDTH}px)`
          },
          zIndex: (theme) => theme.zIndex.appBar
        }}
      >
        <Stack
          alignItems="center"
          direction="row"
          justifyContent="space-between"
          spacing={2}
          sx={{
            minHeight: TOP_NAV_HEIGHT,
            px: 2
          }}
        >
          <Stack
            alignItems="center"
            direction="row"
            spacing={2}
          >
            {!lgUp && (
              <IconButton onClick={onNavOpen}>
                <SvgIcon fontSize="small">
                  <Bars3Icon />
                </SvgIcon>
              </IconButton>
            )}
            <Tooltip title="Search">
              <IconButton>
                <SvgIcon fontSize="small">
                  <MagnifyingGlassIcon />
                </SvgIcon>
              </IconButton>
            </Tooltip>
          </Stack>
          <Stack
            alignItems="center"
            direction="row"
            spacing={2}
          >
            <Tooltip title="Notifications">
            <IconButton onClick={handleNotificationPopoverOpen}>
  <Badge badgeContent={notifications.length} color="warning">
    <SvgIcon fontSize="medium">
      <BellIcon />
    </SvgIcon>
  </Badge>
</IconButton>

            </Tooltip>
            <Avatar {...stringAvatar(user.username)}
              onClick={accountPopover.handleOpen}
              ref={accountPopover.anchorRef}
            />
          </Stack>
        </Stack>
      </Box>
      <AccountPopover
        anchorEl={accountPopover.anchorRef.current}
        open={accountPopover.open}
        onClose={accountPopover.handleClose}
      />
      <NotificationList
        anchorEl={accountPopover.anchorRef.current}
        open={notificationPopoverOpen}
        onClose={handleNotificationPopoverClose}
        notifications={notifications}
      />
    </>
  );
};

TopNav.propTypes = {
  onNavOpen: PropTypes.func
};

const NotificationList = ({ anchorEl, open, onClose, notifications }) => {
  return (
    <Popover
      anchorEl={anchorEl}
      open={open}
      onClose={onClose}
      anchorOrigin={{
        vertical: 'bottom',
        horizontal: 'right',
      }}
      transformOrigin={{
        vertical: 'top',
        horizontal: 'right',
      }}
    >
      <List>
        {notifications.map((notification) => (
          <ListItem key={notification.id}>
            {/* Render an Alert component for each notification */}
            <Alert severity="warning" onClose={() => {}}>
              {notification.message}
            </Alert>
          </ListItem>
        ))}
      </List>
    </Popover>
  );
};