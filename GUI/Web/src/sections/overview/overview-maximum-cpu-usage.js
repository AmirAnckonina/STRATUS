import PropTypes from 'prop-types';
import ArrowDownIcon from '@heroicons/react/24/solid/ArrowDownIcon';
import ArrowUpIcon from '@heroicons/react/24/solid/ArrowUpIcon';
import ArrowUpRightIcon from '@heroicons/react/24/solid/ArrowUpRightIcon';
import UsersIcon from '@heroicons/react/24/solid/UsersIcon';
import { Avatar, Card, CardContent, Stack, SvgIcon, Typography, Box, LinearProgress} from '@mui/material';

export const OverviewMaximumCpuUsage = (props) => {
  const { difference, positive = false, sx, value } = props;

  return (
    <Card sx={sx}>
      <CardContent>
        <Stack
          width="100%"
          alignItems="flex-start"
          direction="row"
          justifyContent="space-between"
          spacing={3}
        >
          <Stack spacing={1}>
            <Typography
              color="text.secondary"
              variant="overline"
            >
              Maximum CPU Usage
            </Typography>
            <Typography variant="h4">
              {value}%
            </Typography>
          </Stack>
          <Avatar
            sx={{
              backgroundColor: 'success.main',
              height: 56,
              width: 56
            }}
          >
            <SvgIcon>
              <ArrowUpRightIcon />
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
              },
              '& .MuiLinearProgress-barColorPrimary': {
                transition: 'none',
                width: `${value}%`,
              },
            }}
          />
        </Box>
      </CardContent>
    </Card>
  );
};

OverviewMaximumCpuUsage.propTypes = {
  difference: PropTypes.number,
  positive: PropTypes.bool,
  value: PropTypes.string.isRequired,
  sx: PropTypes.object
};

