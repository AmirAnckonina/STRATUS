import PropTypes from 'prop-types';
import CurrencyDollarIcon from '@heroicons/react/24/solid/CurrencyDollarIcon';
import PlusIcon from '@heroicons/react/24/solid/PlusIcon';
import { Avatar, Card, CardContent, Stack, SvgIcon, Typography, Box, LinearProgress } from '@mui/material';

export const OverviewMemorySizeUsage = (props) => {
  const { value, sx } = props;

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
              Average Memory Usage
            </Typography>
            <Typography variant="h4">
              {value}%
            </Typography>
          </Stack>
          <Avatar
            sx={{
              backgroundColor: 'primary.main',
              height: 56,
              width: 56
            }}
          >
            <SvgIcon>
              <PlusIcon />
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

OverviewMemorySizeUsage.propTypes = {
  value: PropTypes.string,
  sx: PropTypes.object
};
