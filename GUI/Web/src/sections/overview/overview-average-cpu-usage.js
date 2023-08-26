import PropTypes from 'prop-types';
import ListBulletIcon from '@heroicons/react/24/solid/ListBulletIcon';
import ArrowRightIcon from '@heroicons/react/24/solid/ArrowRightIcon';
import {
  Avatar,
  Box,
  Card,
  CardContent,
  LinearProgress,
  Stack,
  SvgIcon,
  Typography
} from '@mui/material';

export const OverviewAverageCpuUsage = (props) => {
  const { value, sx } = props;

  return (
    <Card sx={sx}>
      <CardContent>
        <Stack
          alignItems="flex-start"
          direction="row"
          justifyContent="space-between"
          spacing={3}
        >
          <Stack spacing={1}>
            <Typography
              color="text.secondary"
              gutterBottom
              variant="overline"
            >
              Average CPU Usage
            </Typography>
            <Typography variant="h4">
              {value}%
            </Typography>
          </Stack>
          <Avatar
            sx={{
              backgroundColor: 'warning.main',
              height: 56,
              width: 56
            }}
          >
            <SvgIcon>
              <ArrowRightIcon />
            </SvgIcon>
          </Avatar>
        </Stack>
        <Box sx={{ mt: 3 }}>
        <LinearProgress
            value={value === 'N/A' ? 0 : value}
            variant="determinate"
            sx={{
              '& .MuiLinearProgress-bar': {
                backgroundColor: 'success.main',
              }
            }}
          />
        </Box>
      </CardContent>
    </Card>
  );
};

OverviewAverageCpuUsage.propTypes = {
  value: PropTypes.number.isRequired,
  sx: PropTypes.object
};
